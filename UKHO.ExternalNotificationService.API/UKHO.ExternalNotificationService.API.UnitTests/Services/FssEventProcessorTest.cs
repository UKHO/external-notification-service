using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.Common.Exceptions;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

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
        public void WhenValidInRequest_ThenReceiveEventType()
        {
            string result = _fssEventProcessor.EventType;

            Assert.That(_fakeCustomCloudEvent.Type, Is.EqualTo(result));
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

            Assert.That("BatchId cannot be blank or null.", Is.EqualTo(result.Errors.Single().Description));
            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenDiscardedBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());
            _fakeFssEventData.BusinessUnit = "test";

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(A<CustomCloudEvent>.Ignored, A<string>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(_fakeFssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
            Assert.That(result.Errors, Is.Null);
            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task WhenUnconfiguredBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;

            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(_fakeCustomCloudEvent.Data)).Returns(_fakeFssEventData);
            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(_fakeFssEventData)).Returns(new ValidationResult());
            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(_fakeCustomCloudEvent, CorrelationId)).Throws<ConfigurationMissingException>();

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
            Assert.That(result.Errors, Is.Null);
            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(_fakeLogger).Where(call =>
                call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error
                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2).ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString()
                == "File share service event data mapping failed for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId} with error:{Message}."
            ).MustHaveHappened(1, Times.Exactly);
        }

        [TestCase("AVCSData")]
        [TestCase("MaritimeSafetyInformation")]
        public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse(string businessUnit)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());
            CustomCloudEvent customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);
            FssEventData fssEventData = CustomCloudEventBase.GetFssEventData(businessUnit);

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.FssEventDataMapping(A<CustomCloudEvent>.Ignored, A<string>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(fssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            ExternalNotificationServiceProcessResponse result = await _fssEventProcessor.Process(customCloudEvent, CorrelationId, cancellationToken);

            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
            Assert.That(result.Errors, Is.Null);
            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
