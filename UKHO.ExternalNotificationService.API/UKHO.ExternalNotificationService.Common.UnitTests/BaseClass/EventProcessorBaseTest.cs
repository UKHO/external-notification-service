using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.BaseClass;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.Common.UnitTests.BaseClass
{
    [TestFixture]
    public class EventProcessorBaseTest
    {
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private EventProcessorBase _eventProcessorBase;
        private FssEventData _fssEventData;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fssEventData = CustomCloudEventBase.GetFssEventData();

            _eventProcessorBase = new EventProcessorBase(_fakeAzureEventGridDomainService);
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenReturnsFssEventData()
        {
            A.CallTo(() => _fakeAzureEventGridDomainService.ConvertObjectTo<FssEventData>(A<object>.Ignored)).Returns(_fssEventData);
            var data = (object)_fssEventData;

            var response = _eventProcessorBase.GetEventData<FssEventData>(data);

            Assert.Multiple(() =>
            {
                Assert.That(response.BatchId, Is.EqualTo(_fssEventData.BatchId));
                Assert.That(response.Links.BatchDetails, Is.EqualTo(_fssEventData.Links.BatchDetails));
            });
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenEventPublishedSuccessfully()
        {
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            var response = _eventProcessorBase.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken);

            Assert.That(response.IsCompleted);
        }

        [Test]
        [TestCase(-1000)]
        [TestCase(0)]
        [TestCase(1000)]
        [TestCase(5000)]
        [TestCase(12000)]
        [Parallelizable(ParallelScope.All)]
        public async Task WhenPublishEventWithDelayAsync_ThenEventPublishedSuccessfullyWithDelay(int millisecondsDelay)
        {
            var cancellationToken = CancellationToken.None;
            var cloudEvent = new CloudEvent("test", "test", new object());
            var stopwatch = new Stopwatch();
            var callToPublishEventAsync = A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            stopwatch.Start();

            await _eventProcessorBase.PublishEventWithDelayAsync(cloudEvent, CorrelationId, millisecondsDelay, cancellationToken);

            stopwatch.Stop();

            callToPublishEventAsync.MustHaveHappened();

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(millisecondsDelay < 0 ? 0 : millisecondsDelay));
        }
    }
}
