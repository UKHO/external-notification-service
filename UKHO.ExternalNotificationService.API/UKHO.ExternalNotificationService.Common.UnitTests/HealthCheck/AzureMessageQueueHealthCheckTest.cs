using System;
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
        private IOptions<SubscriptionStorageConfiguration> _fakeSubscriptionStorageConfiguration;
        private ILogger<AzureMessageQueueHelper> _fakeLogger;
        private IStorageService _fakeStorageService;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private AzureMessageQueueHealthCheck _azureMessageQueueHealthCheck;

        [SetUp]
        public void Setup()
        {
            _fakeSubscriptionStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeLogger = A.Fake<ILogger<AzureMessageQueueHelper>>();
            _fakeStorageService = A.Fake<IStorageService>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();

            _azureMessageQueueHealthCheck = new AzureMessageQueueHealthCheck(_fakeSubscriptionStorageConfiguration, _fakeLogger, _fakeStorageService, _fakeAzureMessageQueueHelper);
        }

        [Test]
        public async Task WhenMessageQueueIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeAzureMessageQueueHelper.CheckMessageQueueHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await _azureMessageQueueHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Healthy));
        }

        [Test]
        public async Task WhenMessageQueueIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeAzureMessageQueueHelper.CheckMessageQueueHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy));

            HealthCheckResult response = await _azureMessageQueueHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }

        [Test]
        public async Task WhenMessageWithException_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeAzureMessageQueueHelper.CheckMessageQueueHealth(A<string>.Ignored, A<string>.Ignored)).Throws(new Exception());

            HealthCheckResult response = await _azureMessageQueueHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }
    }
}
