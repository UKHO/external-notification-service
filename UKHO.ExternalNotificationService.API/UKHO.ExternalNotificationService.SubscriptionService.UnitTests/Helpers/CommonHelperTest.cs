using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Helpers
{
    [TestFixture]
    public class CommonHelperTest
    {
        private ILogger<CallbackService> _fakeLogger;
        public int retryCount = 3;
        private const double SleepDuration = 2;
        const string TestClient = "TestClient";
        private bool _isRetryCalled;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<CallbackService>>();
        }

        [Test]
        public async Task WhenServiceUnavailable_GetRetryPolicy()
        {
            // Arrange 
            IServiceCollection services = new ServiceCollection();
            _isRetryCalled = false;

            services.AddHttpClient(TestClient)
                .AddPolicyHandler(CommonHelper.GetRetryPolicy(_fakeLogger, Common.Logging.EventIds.RetryHttpClientD365CallbackRequest, retryCount, SleepDuration))
                .AddHttpMessageHandler(() => new ServiceUnavailableDelegatingHandler());

            HttpClient configuredClient =
                services
                    .BuildServiceProvider()
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(TestClient);

            // Act
            HttpResponseMessage result = await configuredClient.GetAsync("https://testretry.com");

            // Assert
            Assert.False(_isRetryCalled);
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);

        }

        [Test]
        public async Task WhenInternalServerError_GetRetryPolicy()
        {
            // Arrange 
            IServiceCollection services = new ServiceCollection();
            _isRetryCalled = false;
            retryCount = 1;

            services.AddHttpClient(TestClient)
                .AddPolicyHandler(CommonHelper.GetRetryPolicy(_fakeLogger, Common.Logging.EventIds.RetryHttpClientD365CallbackRequest, retryCount, SleepDuration))
                .AddHttpMessageHandler(() => new InternalServerDelegatingHandler());

            HttpClient configuredClient =
                services
                    .BuildServiceProvider()
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(TestClient);

            // Act
            HttpResponseMessage result = await configuredClient.GetAsync("https://testretry.com");

            // Assert
            Assert.False(_isRetryCalled);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        }

    }
}
