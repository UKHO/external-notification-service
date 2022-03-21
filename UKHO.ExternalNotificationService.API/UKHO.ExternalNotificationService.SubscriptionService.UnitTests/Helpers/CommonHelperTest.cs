using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

            HttpClient configuredClient = CreateClient(services);

            HttpResponseMessage result = await configuredClient.GetAsync("https://testretry.com");

            Assert.False(_isRetryCalled);
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
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

            HttpClient configuredClient = CreateClient(services);

            HttpResponseMessage result = await configuredClient.GetAsync("https://testretry.com");

            Assert.False(_isRetryCalled);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Test]
        public void WhenValidGetExternalNotificationEntityEventThenReturnSuccessResponse()
        {            
            SubscriptionRequestResult subscriptionRequestResult = GetSubscriptionSuccessRequestResult();
            bool fakeIsActive = true;
            int fakeSuccessStatusCode = 1000001;
            ExternalNotificationEntity response = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, fakeIsActive, fakeSuccessStatusCode);
            Assert.IsNotNull(response);
         }

        [Test]
        public void WhenInvalidGetExternalNotificationEntityEventThenReturnFailResponse()
        {
            SubscriptionRequestResult subscriptionRequestResult = GetSubscriptionFailureRequestResult();
            bool fakeIsActive = false;
            int fakeFailureStatusCode = 1000002;
            ExternalNotificationEntity response = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, fakeIsActive, fakeFailureStatusCode);
            Assert.IsNotNull(response);
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
                ProvisioningState="Succeeded",                
                WebhookUrl = "https://testurl.com",
                ErrorMessage= null
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
                ErrorMessage = null
            };
        }
    }
}
