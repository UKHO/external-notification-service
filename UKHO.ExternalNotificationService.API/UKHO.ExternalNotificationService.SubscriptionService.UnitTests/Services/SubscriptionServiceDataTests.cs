using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
            A.CallTo(() => _fakeAzureEventGridDomainService.CreateOrUpdateSubscription(A<SubscriptionRequestMessage>.Ignored, A<CancellationToken>.Ignored));

            var response = await _fakeSubscriptionServiceData.CreateOrUpdateSubscription(GetSubscriptionRequestMessage(), cancellationToken);
            Assert.IsInstanceOf<string>(response);
        }

        private static SubscriptionRequestMessage GetSubscriptionRequestMessage()
        {
            return new SubscriptionRequestMessage()
            {
                CorrelationId = "",
                NotificationType = "",
                IsActive = true,
                NotificationTypeTopicName = "",
                SubscriptionId = "1",
                WebhookUrl = "https://testurl.com"

            };
        }
    }
}
