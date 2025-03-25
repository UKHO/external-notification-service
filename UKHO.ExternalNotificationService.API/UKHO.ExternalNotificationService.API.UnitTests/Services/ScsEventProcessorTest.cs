using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Monitoring;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class ScsEventProcessorTest
    {
        private IScsEventValidationAndMappingService _fakeScsEventValidationAndMappingService;
        private ILogger<ScsEventProcessor> _fakeLogger;
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private IOptions<EventProcessorConfiguration> _fakeEventProcessorConfiguration;
        private ScsEventProcessor _scsEventProcessor;
        private CustomCloudEvent _fakeCustomCloudEvent;
        private ScsEventData _fakeScsEventData;
        private IConfiguration _configuration;
        private IAddsMonitoringService _fakeAddsMonitoringService;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeScsEventData = CustomCloudEventBase.GetScsEventData();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeCustomCloudEvent.Data = _fakeScsEventData;
            _fakeCustomCloudEvent.Type = "uk.gov.UKHO.catalogue.productUpdated.v1";

            _fakeScsEventValidationAndMappingService = A.Fake<IScsEventValidationAndMappingService>();
            _fakeLogger = A.Fake<ILogger<ScsEventProcessor>>();
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fakeEventProcessorConfiguration = A.Fake<IOptions<EventProcessorConfiguration>>();

            _fakeAddsMonitoringService = A.Fake<IAddsMonitoringService>();

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = 0;

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ADDSMonitoringEnabled", bool.FalseString }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _scsEventProcessor = CreateScsEventProcessor(_configuration);
        }

        private ScsEventProcessor CreateScsEventProcessor(IConfiguration configuration) =>
            new(_fakeScsEventValidationAndMappingService,
                _fakeLogger, _fakeAzureEventGridDomainService, _fakeEventProcessorConfiguration,
                configuration, _fakeAddsMonitoringService);

        [Test]
        public void WhenValidPayloadInRequest_ThenReceiveEventType()
        {
            var result = _scsEventProcessor.EventType;

            Assert.That(result, Is.EqualTo(_fakeCustomCloudEvent.Type));
        }

        [Test]
        public async Task WhenInvalidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            var validationMessage = new ValidationFailure("ProductType", "ProductType cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.OK.ToString()
            };

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored))
                            .Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            var result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Single().Description, Is.EqualTo("ProductType cannot be blank or null."));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            A.CallTo(() =>
                    _fakeAddsMonitoringService.StopProcessAsync(A<AddsData>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored)).Returns(_fakeScsEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            var result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });

            A.CallTo(() =>
                    _fakeAddsMonitoringService.StopProcessAsync(A<AddsData>.Ignored, CorrelationId, cancellationToken))
                .MustNotHaveHappened();
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
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = configuredDelay;

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored)).Returns(_fakeScsEventData);

            var callToPublishEventAsync = A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            stopwatch.Stop();

            callToPublishEventAsync.MustHaveHappened();

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
                Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(expectedDelay * 1000));
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

            var result = _scsEventProcessor.DelayInMilliseconds;

            Assert.That(result, Is.EqualTo(expectedDelay * 1000));
        }

        [Test]
        public async Task WhenADDSMonitoringEnabled_ThenStopADDSMonitoringProcess()
        {
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ADDSMonitoringEnabled", bool.TrueString }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _scsEventProcessor = CreateScsEventProcessor(_configuration);

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored)).Returns(_fakeScsEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            var result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Errors, Is.Empty);
            });

            A.CallTo(() =>
                    _fakeAddsMonitoringService.StopProcessAsync(A<AddsData>.Ignored, CorrelationId, cancellationToken))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenInvalidPayloadInRequestAndADDSMonitoringEnabled_ThenDoNotStopADDSMonitoringProcess()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ADDSMonitoringEnabled", bool.TrueString }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _scsEventProcessor = CreateScsEventProcessor(_configuration);
            var validationMessage = new ValidationFailure("ProductType", "ProductType cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.OK.ToString()
            };

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            var result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Single().Description, Is.EqualTo("ProductType cannot be blank or null."));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            A.CallTo(() =>
                    _fakeAddsMonitoringService.StopProcessAsync(A<AddsData>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
