using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;
using UKHO.ExternalNotificationService.Common.UnitTests.BaseClass;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class FssEventProcessorTest
    {
        private IFssEventValidationAndMappingService _fakeFssEventValidationAndMappingService;
        private ILogger<FssEventProcessor> _fakeLogger;
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private FssEventProcessor _fssEventProcessor;
        private CustomCloudEvent _fakeCustomCloudEvent;
        private FssEventData _fakeFssEventData;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeFssEventValidationAndMappingService = A.Fake<IFssEventValidationAndMappingService>();
            _fakeLogger = A.Fake<ILogger<FssEventProcessor>>();
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeFssEventData = CustomCloudEventBase.GetFssEventData();

            _fssEventProcessor = new FssEventProcessor(_fakeFssEventValidationAndMappingService,
                                                       _fakeLogger, _fakeAzureEventGridDomainService);
        }

        [Test]
        public void  WhenValidInRequest_ThenReceiveEventType()
        {
            string result = _fssEventProcessor.EventType;

            Assert.AreEqual(_fakeCustomCloudEvent.Type, result);
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

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.AreEqual("BatchId cannot be blank or null.", result.Errors.Single().Description);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task WhenInvalidBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());
            _fakeFssEventData.BusinessUnit = "test";

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(A<CustomCloudEvent>.Ignored, A<string>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(_fakeFssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.AreEqual("Invalid business unit in an event.", result.Errors.Single().Description);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(A<CustomCloudEvent>.Ignored, A<string>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(_fakeFssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.IsNull(result.Errors);
        }
    }
}
