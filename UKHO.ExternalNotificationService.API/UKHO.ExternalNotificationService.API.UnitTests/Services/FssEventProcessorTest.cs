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
            var result = _fssEventProcessor.EventType;

            Assert.That(result, Is.EqualTo(_fakeCustomCloudEvent.Type));
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

            var result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Single().Description, Is.EqualTo("BatchId cannot be blank or null."));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        public async Task WhenDiscardedBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());
            _fakeFssEventData.BusinessUnit = "test";

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<FssEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<FssEventData>(A<object>.Ignored)).Returns(_fakeFssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            var result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });
            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task WhenUnconfiguredBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            var cancellationToken = CancellationToken.None;

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<FssEventData>(_fakeCustomCloudEvent.Data)).Returns(_fakeFssEventData);

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(_fakeFssEventData)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<FssEventData>>.Ignored)).Throws<ConfigurationMissingException>();

            var result = await _fssEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });
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
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());
            var customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);
            var fssEventData = CustomCloudEventBase.GetFssEventData(businessUnit);

            A.CallTo(() => _fakeFssEventValidationAndMappingService.ValidateFssEventData(A<FssEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeFssEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<FssEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<FssEventData>(A<object>.Ignored)).Returns(fssEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored));

            var result = await _fssEventProcessor.Process(customCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });
            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
