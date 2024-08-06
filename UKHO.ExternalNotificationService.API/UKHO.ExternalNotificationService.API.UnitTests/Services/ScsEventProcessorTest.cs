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
    public class ScsEventProcessorTest
    {
        private IScsEventValidationAndMappingService _fakeScsEventValidationAndMappingService;
        private ILogger<ScsEventProcessor> _fakeLogger;
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private IOptions<EventProcessorConfiguration> _fakeEventProcessorConfiguration;
        private ScsEventProcessor _scsEventProcessor;
        private CustomCloudEvent _fakeCustomCloudEvent;
        private ScsEventData _fakeScsEventData;
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

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = 0;  

            _scsEventProcessor = new ScsEventProcessor(_fakeScsEventValidationAndMappingService,
                                                       _fakeLogger, _fakeAzureEventGridDomainService, _fakeEventProcessorConfiguration);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReceiveEventType()
        {
            string result = _scsEventProcessor.EventType;

            Assert.That(_fakeCustomCloudEvent.Type, Is.EqualTo(result));
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

            ExternalNotificationServiceProcessResponse result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId);

            Assert.That("ProductType cannot be blank or null.", Is.EqualTo(result.Errors.Single().Description));
            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
        }

        [Test]
       public async Task WhenValidPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored)).Returns(new ValidationResult());
            
            A.CallTo(() => _fakeScsEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored)).Returns(_fakeScsEventData);

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            ExternalNotificationServiceProcessResponse result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
            Assert.That(result.Errors, Is.Empty);
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
            CloudEvent cloudEvent = new("test", "test", new object());

            _fakeEventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds = configuredDelay; 

            A.CallTo(() => _fakeScsEventValidationAndMappingService.ValidateScsEventData(A<ScsEventData>.Ignored)).Returns(new ValidationResult());

            A.CallTo(() => _fakeScsEventValidationAndMappingService.MapToCloudEvent(A<CloudEventCandidate<ScsEventData>>.Ignored)).Returns(cloudEvent);

            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<ScsEventData>(A<object>.Ignored)).Returns(_fakeScsEventData);

            IReturnValueArgumentValidationConfiguration<Task> callToPublishEventAsync = A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            Stopwatch stopwatch = new();

            stopwatch.Start();

            ExternalNotificationServiceProcessResponse result = await _scsEventProcessor.Process(_fakeCustomCloudEvent, CorrelationId, cancellationToken);

            stopwatch.Stop();

            callToPublishEventAsync.MustHaveHappened();

            Assert.That(HttpStatusCode.OK, Is.EqualTo(result.StatusCode));
            Assert.That(result.Errors, Is.Empty);

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(expectedDelay * 1000));
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

            int result = _scsEventProcessor.DelayInMilliseconds;

            Assert.That(expectedDelay * 1000, Is.EqualTo(result));
        }
        
    }
}
