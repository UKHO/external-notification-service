using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Services
{
    [TestFixture]
    public class SubscriptionServiceDataTests
    {
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private ILogger<SubscriptionServiceData> _fakeLogger;
        private SubscriptionServiceData _fakeSubscriptionServiceData;

        [SetUp]
        public void Setup()
        {
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fakeLogger = A.Fake<ILogger<SubscriptionServiceData>>();

            _fakeSubscriptionServiceData = new SubscriptionServiceData(_fakeAzureEventGridDomainService, _fakeLogger);
        }

        [Test]
        public async Task WhenCreateOrUpdateSubscriptionThenCreateSubscription()
        {
            CancellationToken cancellationToken = CancellationToken.None;

            string response = await _fakeSubscriptionServiceData.CreateOrUpdateSubscription(GetSubscriptionRequestMessage(), cancellationToken);

            A.CallTo(() => _fakeAzureEventGridDomainService.CreateOrUpdateSubscription(A<SubscriptionRequestMessage>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsInstanceOf<string>(response);
        }

        private static SubscriptionRequestMessage GetSubscriptionRequestMessage()
        {
            return new SubscriptionRequestMessage()
            {
                D365CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                NotificationType = "Data test",
                IsActive = true,
                NotificationTypeTopicName = "test-Topic-type",
                SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                WebhookUrl = "https://testurl.com"

            };
        }
    }
}
