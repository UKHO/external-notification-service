using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.API.Services;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class WebhookServiceTest
    {
        private List<IEventProcessor> _fakeEventProcessors;
        private WebhookService _webhookService;
        private FssEventProcessor _fakeFssEventProcessor;

        [SetUp]
        public void Setup()
        {
            _fakeFssEventProcessor = A.Fake<FssEventProcessor>();
            _fakeEventProcessors = new List<IEventProcessor>
            {
                _fakeFssEventProcessor
            };

            _webhookService = new WebhookService(_fakeEventProcessors);
        }

        [Test]
        public void WhenInvalidValidEventTypeInRequest_ThenReturnNullValue()
        {
            var result = _webhookService.GetProcessor("uk.gov.UKHO.NewFilesPublished.v1");

            Assert.IsNull(result);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReturnIEventProcessorService()
        {
            var result = _webhookService.GetProcessor("uk.gov.UKHO.FileShareService.NewFilesPublished.v1");

            Assert.IsNotNull(result);
        }
    }
}
