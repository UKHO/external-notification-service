using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class ScsS100EventProcessorTest
    {
        private IScsS100EventValidationAndMappingService _fakeScsS100EventValidationAndMappingService;
        private ILogger<ScsS100EventProcessor> _fakeLogger;
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private IOptions<EventProcessorConfiguration> _fakeEventProcessorConfiguration;
        private ScsS100EventProcessor _scsS100EventProcessor;
        private CustomCloudEvent _fakeCustomCloudEvent;
        private ScsEventData _fakeScsEventData;
        public const string CorrelationId = "c2f5e4c4-9d6b-4bd0-b6e5-1a0e54e4d111";

        [SetUp]
        public void Setup()
        {
            _fakeScsEventData = CustomCloudEventBase.GetScsEventData();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeCustomCloudEvent.Data = _fakeScsEventData;

            _fakeScsS100EventValidationAndMappingService = A.Fake<IScsS100EventValidationAndMappingService>();
            _fakeLogger = A.Fake<ILogger<ScsS100EventProcessor>>();
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fakeEventProcessorConfiguration = A.Fake<IOptions<EventProcessorConfiguration>>();

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = 0;

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored))
                .Returns(_fakeScsEventData);

            _scsS100EventProcessor = new ScsS100EventProcessor(
                _fakeScsS100EventValidationAndMappingService,
                _fakeLogger,
                _fakeAzureEventGridDomainService,
                _fakeEventProcessorConfiguration);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReceiveEventType()
        {
            string result = _scsS100EventProcessor.EventType;

            Assert.That(result, Is.EqualTo(EventProcessorTypes.SCSS100));
        }

        [Test]
        public async Task WhenInvalidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            var validationMessage = new ValidationFailure("ProductType", "ProductType cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.OK.ToString()
            };

            A.CallTo(() => _fakeScsS100EventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            ExternalNotificationServiceProcessResponse result =
                await _scsS100EventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors.Single().Description, Is.EqualTo("ProductType cannot be blank or null."));
            });

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(A<CloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("testsource", "testtype", new object());

            A.CallTo(() => _fakeScsS100EventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _fakeScsS100EventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored))
                .Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            ExternalNotificationServiceProcessResponse result =
                await _scsS100EventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        [TestCase(10, 10)]
        [TestCase(12, 10)]
        [TestCase(60, 10)]
        public async Task WhenValidPayloadInRequestAndConfiguredDelay_ThenReceiveSuccessfulResponse(int configuredDelay, int expectedDelay)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("testsource", "testtype", new object());

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = configuredDelay;

            A.CallTo(() => _fakeScsS100EventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _fakeScsS100EventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored))
                .Returns(cloudEvent);

            IReturnValueArgumentValidationConfiguration<Task> publishCall =
                A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            Stopwatch sw = new();
            sw.Start();

            ExternalNotificationServiceProcessResponse result =
                await _scsS100EventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            sw.Stop();

            publishCall.MustHaveHappened();

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
                Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(expectedDelay * 1000));
            });
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        [TestCase(10, 10)]
        [TestCase(12, 10)]
        [TestCase(60, 10)]
        public void WhenDelayInMillisecondsIsAccessed_ThenReturnedSuccessfully(int configuredDelay, int expectedDelay)
        {
            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = configuredDelay;

            int result = _scsS100EventProcessor.DelayInMilliseconds;

            Assert.That(result, Is.EqualTo(expectedDelay * 1000));
        }
    }
}           
