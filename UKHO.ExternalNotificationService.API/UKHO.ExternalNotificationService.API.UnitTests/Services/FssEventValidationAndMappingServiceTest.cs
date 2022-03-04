using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class FssEventValidationAndMappingServiceTest
    {
        private IFssEventDataValidator _fakeFssEventDataValidator;
        private IOptions<FssDataMappingConfiguration> _fakeFssDataMappingConfiguration;
        private FssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private FssEventData _fakeFssEventData;
        private CustomEventGridEvent _fakeCustomEventGridEvent;

        [SetUp]
        public void Setup()
        {
            _fakeFssEventData = GetFssEventData();
            _fakeCustomEventGridEvent = GetCustomEventGridEvent();
            _fakeFssEventDataValidator = A.Fake<IFssEventDataValidator>();
            _fakeFssDataMappingConfiguration = A.Fake<IOptions<FssDataMappingConfiguration>>();
            _fakeFssDataMappingConfiguration.Value.Source = "fss-Test";
            _fakeFssDataMappingConfiguration.Value.ExistingHostName = "files.admiralty.co.uk";
            _fakeFssDataMappingConfiguration.Value.ReplacingHostName = "admiralty.test/fss";
            _fakeFssDataMappingConfiguration.Value.BusinessUnit = "AVCSTest";
            _fakeFssDataMappingConfiguration.Value.Type = "uk.co.admiralty.fss.test";

            _fssEventValidationAndMappingService = new FssEventValidationAndMappingService(_fakeFssEventDataValidator, _fakeFssDataMappingConfiguration);
        }

        #region ValidateFssEventData
        [Test]
        public async Task WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("BatchId", "BatchId cannot be blank or null.")}));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(new FssEventData());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("BatchId cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(_fakeFssEventData);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region FssEventDataMapping
        [Test]
        public void WhenValidFssEventDataMappingRequest_ThenReturnCloudEvent()
        {
            string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";
            string batchDetailsUri = "https://admiralty.test/fss/batch/83d08093-7a67-4b3a-b431-92ba42feaea0";

            CloudEvent result =  _fssEventValidationAndMappingService.FssEventDataMapping(_fakeCustomEventGridEvent, correlationId);

            string data = Encoding.ASCII.GetString(result.Data);
            FssEventData cloudEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            Assert.AreEqual(_fakeFssDataMappingConfiguration.Value.Type, result.Type);
            Assert.AreEqual(_fakeFssDataMappingConfiguration.Value.Source, result.Source);
            Assert.AreEqual(batchDetailsUri, cloudEventData.Links.BatchDetails.Href);
            Assert.AreEqual(batchDetailsUri + "/status", cloudEventData.Links.BatchStatus.Href);
            Assert.AreEqual(batchDetailsUri + "/files/AVCS_S631-1_Update_Wk45_21_Only.zip", cloudEventData.Files.FirstOrDefault().Links.Get.Href);
        }
        #endregion

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
