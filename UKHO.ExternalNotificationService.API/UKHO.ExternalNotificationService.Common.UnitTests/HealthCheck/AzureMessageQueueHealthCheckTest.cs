
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.HealthCheck;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Storage;

namespace UKHO.ExternalNotificationService.Common.UnitTests.HealthCheck
{
    public class AzureMessageQueueHealthCheckTest
    {
        private IOptions<EnsFulfilmentStorageConfiguration> _fakeEnsFulfilmentStorageConfiguration;
        private ILogger<AzureMessageQueueHelper> _fakeLogger;
        private IStorageService _fakeStorageService;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private AzureMessageQueueHealthCheck _azureMessageQueueHealthCheck;


        [SetUp]
        public void Setup()
        {
            _fakeEnsFulfilmentStorageConfiguration = A.Fake<IOptions<EnsFulfilmentStorageConfiguration>>();
            _fakeLogger = A.Fake<ILogger<AzureMessageQueueHelper>>();
            _fakeStorageService = A.Fake<IStorageService>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();

            _azureMessageQueueHealthCheck = new AzureMessageQueueHealthCheck(_fakeEnsFulfilmentStorageConfiguration,_fakeLogger,_fakeStorageService,_fakeAzureMessageQueueHelper);
        }

        [Test]
        public async Task WhenMessageQueueIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeAzureMessageQueueHelper.CheckMessageQueueHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            var response = await _azureMessageQueueHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenMessageQueueIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeAzureMessageQueueHelper.CheckMessageQueueHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy));

            var response = await _azureMessageQueueHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
