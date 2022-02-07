
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
    [TestFixture]
    public class AzureBlobStorageHealthCheckTest
    {
        private IAzureBlobStorageHelper _fakeAzureBlobStorageHelper;
        private IStorageService _fakeStorageService;
        private IOptions<SubscriptionStorageConfiguration> _fakeSubscriptionStorageConfiguration;
        private ILogger<AzureBlobStorageHealthCheck> _fakeLogger;
        private AzureBlobStorageHealthCheck _azureBlobStorageHealthCheck;
        

        [SetUp]
        public void Setup()
        {
            _fakeAzureBlobStorageHelper = A.Fake<IAzureBlobStorageHelper>();
            _fakeStorageService= A.Fake<IStorageService>();
            _fakeSubscriptionStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeLogger = A.Fake<ILogger<AzureBlobStorageHealthCheck>>();

            _azureBlobStorageHealthCheck = new AzureBlobStorageHealthCheck(_fakeAzureBlobStorageHelper, _fakeStorageService, _fakeSubscriptionStorageConfiguration, _fakeLogger);
        }

        [Test]
        public async Task WhenBlobStorageIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeAzureBlobStorageHelper.CheckBlobContainerHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            var response = await _azureBlobStorageHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenBlobStorageIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeAzureBlobStorageHelper.CheckBlobContainerHealth(A<string>.Ignored, A<string>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy));

            var response = await _azureBlobStorageHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
