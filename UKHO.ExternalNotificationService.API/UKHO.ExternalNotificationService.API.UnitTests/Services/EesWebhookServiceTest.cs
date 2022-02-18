using Azure.Messaging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class EesWebhookServiceTest
    {
        private EesWebhookService _eesWebhookService;

        [SetUp]
        public void Setup()
        {
            _eesWebhookService = new EesWebhookService();
        }

        [Test]
        public void WhenInvalidJsonContentInRequest_ThenReturnNullValue()
        {
            string fakeCacheJson = "{key: \\\"data\\\",value: \\\"test\\\"}";

            CloudEvent result = _eesWebhookService.TryGetCloudEventMessage(fakeCacheJson.ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void WhenValidJsonContentInRequest_ThenReturnCloudEventObject()
        {
            string fakeCacheJson = "{\"subject\": \"test\"}";

            CloudEvent result = _eesWebhookService.TryGetCloudEventMessage(fakeCacheJson.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Subject);
        }
    }
}
