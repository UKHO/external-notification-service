using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class EventProcessorFactoryTest
    {
        private List<IEventProcessor> _fakeEventProcessors;
        private EventProcessorFactory _eventProcessorFactory;
        private FssEventProcessor _fakeFssEventProcessor;

        [SetUp]
        public void Setup()
        {
            _fakeFssEventProcessor = A.Fake<FssEventProcessor>();
            _fakeEventProcessors = new List<IEventProcessor>
            {
                _fakeFssEventProcessor
            };

            _eventProcessorFactory = new EventProcessorFactory(_fakeEventProcessors);
        }

        [Test]
        public void WhenInvalidEventTypeInRequest_ThenReturnNullValue()
        {
            IEventProcessor result = _eventProcessorFactory.GetProcessor("uk.gov.UKHO.NewFilesPublished.v1");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReturnIEventProcessorService()
        {
            IEventProcessor result = _eventProcessorFactory.GetProcessor("uk.gov.UKHO.FileShareService.NewFilesPublished.v1");

            Assert.That(result, Is.Not.Null);
        }
    }
}
