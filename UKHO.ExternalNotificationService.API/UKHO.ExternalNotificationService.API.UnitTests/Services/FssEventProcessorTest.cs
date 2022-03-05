using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class FssEventProcessorTest
    {
        private IFssEventValidationAndMappingService _fakeFssEventValidationAndMappingService;
        private IOptions<FssDataMappingConfiguration> _fakeFssDataMappingConfiguration;
        private ILogger<FssEventProcessor> _fakeLogger;
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private FssEventProcessor _fssEventProcessor;
        private CustomEventGridEvent _fakeCustomEventGridEvent;
        public const string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeFssDataMappingConfiguration = A.Fake<IOptions<FssDataMappingConfiguration>>();
            _fakeFssEventValidationAndMappingService = A.Fake<IFssEventValidationAndMappingService>();
            _fakeLogger = A.Fake<ILogger<FssEventProcessor>>();
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fakeCustomEventGridEvent = GetCustomEventGridEvent();
            _fakeFssDataMappingConfiguration.Value.BusinessUnit = "AVCSData";

            _fssEventProcessor = new FssEventProcessor(_fakeFssEventValidationAndMappingService,_fakeFssDataMappingConfiguration,
                                                       _fakeLogger, _fakeAzureEventGridDomainService);
        }

        [Test]
        public async Task WhenInvalidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            var validationMessage = new ValidationFailure("BatchId", "BatchId cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.OK.ToString()
            };

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored))
                            .Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomEventGridEvent, correlationId);

            Assert.AreEqual("BatchId cannot be blank or null.", result.Errors.Single().Description);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task WhenInvalidBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssDataMappingConfiguration.Value.BusinessUnit = "test";

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored))
                            .Returns(new ValidationResult());

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomEventGridEvent, correlationId);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(A<CustomEventGridEvent>.Ignored, A<string>.Ignored))
                            .Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, cancellationToken))
                            .Returns(true);

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomEventGridEvent, correlationId);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.IsNull(result.Errors);
        }

        private static CustomEventGridEvent GetCustomEventGridEvent()
        {
            return new CustomEventGridEvent()
            {
                Type = "uk.gov.UKHO.FileShareService.NewFilesPublished.v1",
                Source = "https://files.admiralty.co.uk",
                Id = "49c67cca-9cca-4655-a38e-583693af55ea",
                Subject = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                DataContentType = "application/json",
                Data = GetFssEventData(),
                Time = "2021-11-09T14:52:28+00:00"
            };
        }

        private static FssEventData GetFssEventData()
        {
            Link linkBatchDetails = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };
            Link linkBatchStatus = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };

            FileLinks fileLinks = new()
            {
                Get = new Link() { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
            };

            BatchLinks links = new()
            {
                BatchDetails = linkBatchDetails,
                BatchStatus = linkBatchStatus
            };

            return new FssEventData()
            {
                Links = links,
                BusinessUnit = "AVCSData",
                Attributes = new List<Attribute> { },
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                BatchPublishedDate = DateTime.UtcNow,
                Files = new File[] {new() { MIMEType= "application/zip",
                                                                    FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                                                                    FileSize=99073923,
                                                                    Hash="yNpJTWFKhD3iasV8B/ePKw==",
                                                                    Attributes=new List<Attribute> {},
                                                                    Links = fileLinks   }}
            };
        }
    }
}
