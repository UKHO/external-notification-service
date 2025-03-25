using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Helpers
{
    [TestFixture]
    public class CommonHelperTest
    {
        private ILogger<CallbackService> _fakeLogger;
        private int _retryCount = 3;
        private const double SleepDuration = 2;
        private const string TestClient = "TestClient";
        private bool _isRetryCalled;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<CallbackService>>();
        }

        [Test]
        public async Task WhenServiceUnavailable_ThenGetRetryPolicy()
        {
            IServiceCollection services = new ServiceCollection();
            _isRetryCalled = false;

            services.AddHttpClient(TestClient)
                .AddPolicyHandler(CommonHelper.GetRetryPolicy(_fakeLogger, _retryCount, SleepDuration))
                .AddHttpMessageHandler(() => new ServiceUnavailableDelegatingHandler());

            var configuredClient = CreateClient(services);

            var result = await configuredClient.GetAsync("https://testretry.com");

            Assert.Multiple(() =>
            {
                Assert.That(_isRetryCalled, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            });
        }

        [Test]
        public async Task WhenInternalServerError_ThenGetRetryPolicy()
        {
            IServiceCollection services = new ServiceCollection();
            _isRetryCalled = false;
            _retryCount = 1;

            services.AddHttpClient(TestClient)
                .AddPolicyHandler(CommonHelper.GetRetryPolicy(_fakeLogger, _retryCount, SleepDuration))
                .AddHttpMessageHandler(() => new InternalServerDelegatingHandler());

            var configuredClient = CreateClient(services);

            var result = await configuredClient.GetAsync("https://testretry.com");

            Assert.Multiple(() =>
            {
                Assert.That(_isRetryCalled, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }

        [Test]
        public void WhenValidGetExternalNotificationEntityEventThenReturnSuccessResponse()
        {
            var subscriptionRequestResult = GetSubscriptionSuccessRequestResult();
            const bool isActive = true;
            const int successStatusCode = 1000001;

            var response = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, isActive, successStatusCode);
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public void WhenInvalidGetExternalNotificationEntityEventThenReturnFailResponse()
        {
            var subscriptionRequestResult = GetSubscriptionFailureRequestResult();
            const bool isActive = false;
            const int failureStatusCode = 1000002;

            var response = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, isActive, failureStatusCode);
            Assert.That(response, Is.Not.Null);
        }

        private static HttpClient CreateClient(IServiceCollection services)
        {
            return services
                    .BuildServiceProvider()
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(TestClient);
        }

        private static SubscriptionRequestResult GetSubscriptionSuccessRequestResult()
        {
            return new SubscriptionRequestResult(new SubscriptionRequestMessage())
            {
                SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                NotificationType = "Data test",
                ProvisioningState = "Succeeded",
                WebhookUrl = "https://testurl.com",
                ErrorMessage = null
            };
        }

        private static SubscriptionRequestResult GetSubscriptionFailureRequestResult()
        {
            return new SubscriptionRequestResult(new SubscriptionRequestMessage())
            {
                SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                NotificationType = "Data test",
                ProvisioningState = "Failed",
                WebhookUrl = "https://testurl.com",
                ErrorMessage = "Operation returned invalid"
            };
        }
    }
}
