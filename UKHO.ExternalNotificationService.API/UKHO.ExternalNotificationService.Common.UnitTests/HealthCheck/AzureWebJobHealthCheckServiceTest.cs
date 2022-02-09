
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.HealthCheck;
using UKHO.ExternalNotificationService.Common.Helpers;

namespace UKHO.ExternalNotificationService.Common.UnitTests.HealthCheck
{
    public class AzureWebJobHealthCheckServiceTest
    {
        private IWebJobAccessKeyProvider _fakeWebJobAccessKeyProvider;
        private IWebHostEnvironment _fakeWebHostEnvironment;
        private IAzureWebJobsHelper _fakeAzureWebJobsHelper;
        private AzureWebJobHealthCheckService azureWebJobHealthCheckService;

        [SetUp]
        public void Setup()
        {
            _fakeWebJobAccessKeyProvider = A.Fake<IWebJobAccessKeyProvider>();
            _fakeWebHostEnvironment = A.Fake<IWebHostEnvironment>();
            _fakeAzureWebJobsHelper = A.Fake<IAzureWebJobsHelper>();

            azureWebJobHealthCheckService = new AzureWebJobHealthCheckService(_fakeWebJobAccessKeyProvider, _fakeWebHostEnvironment, _fakeAzureWebJobsHelper);
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsNotRunning_ThenReturnUnhealthy()
        {
            A.CallTo(() => _fakeAzureWebJobsHelper.CheckWebJobsHealth(A<WebJobDetails>.Ignored))
               .Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Azure webjob is unhealthy"));

            var response = await azureWebJobHealthCheckService.CheckHealthAsync();

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsRunning_ThenReturnHealthy()
        {
            A.CallTo(() => _fakeAzureWebJobsHelper.CheckWebJobsHealth(A<WebJobDetails>.Ignored))
               .Returns(new HealthCheckResult(HealthStatus.Healthy, "Azure webjob is healthy"));

            var response = await azureWebJobHealthCheckService.CheckHealthAsync();

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }
    }
}
