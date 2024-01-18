using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.Common.UnitTests.HealthCheck
{
    public class AzureWebJobHealthCheckServiceTest
    {
        private IWebJobAccessKeyProvider _fakeWebJobAccessKeyProvider;
        private IWebHostEnvironment _fakeWebHostEnvironment;
        private IAzureWebJobHelper _fakeAzureWebJobHelper;
        private IAzureWebJobHealthCheckService _azureWebJobHealthCheckService;

        [SetUp]
        public void Setup()
        {
            _fakeWebJobAccessKeyProvider = A.Fake<IWebJobAccessKeyProvider>();
            _fakeWebHostEnvironment = A.Fake<IWebHostEnvironment>();
            _fakeAzureWebJobHelper = A.Fake<IAzureWebJobHelper>();

            _azureWebJobHealthCheckService = new AzureWebJobHealthCheckService(_fakeWebJobAccessKeyProvider, _fakeWebHostEnvironment, _fakeAzureWebJobHelper);
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsNotRunning_ThenReturnUnhealthy()
        {
            A.CallTo(() => _fakeAzureWebJobHelper.CheckWebJobsHealth(A<WebJobDetails>.Ignored))
               .Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Azure webjob is unhealthy"));

            HealthCheckResult response = await _azureWebJobHealthCheckService.CheckHealthAsync();

            Assert.That(HealthStatus.Unhealthy, Is.EqualTo(response.Status));
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsRunning_ThenReturnHealthy()
        {
            A.CallTo(() => _fakeAzureWebJobHelper.CheckWebJobsHealth(A<WebJobDetails>.Ignored))
               .Returns(new HealthCheckResult(HealthStatus.Healthy, "Azure webjob is healthy"));

            HealthCheckResult response = await _azureWebJobHealthCheckService.CheckHealthAsync();

            Assert.That(HealthStatus.Healthy, Is.EqualTo(response.Status));
        }
    }
}
