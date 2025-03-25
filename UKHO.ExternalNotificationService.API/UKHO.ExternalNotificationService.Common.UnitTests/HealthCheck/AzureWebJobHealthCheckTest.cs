using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.Common.UnitTests.HealthCheck
{
    public class AzureWebJobHealthCheckTest
    {
        private IAzureWebJobHealthCheckService _fakeAzureWebJobHealthCheckService;
        private AzureWebJobHealthCheck _azureWebJobHealthCheck;
        private ILogger<AzureWebJobHealthCheck> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<AzureWebJobHealthCheck>>();
            _fakeAzureWebJobHealthCheckService = A.Fake<IAzureWebJobHealthCheckService>();

            _azureWebJobHealthCheck = new AzureWebJobHealthCheck(_fakeAzureWebJobHealthCheckService, _fakeLogger);
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsRunning_ThenAzureWebJobsIsHealthy()
        {
            A.CallTo(() => _fakeAzureWebJobHealthCheckService.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy, "Azure webjob is healthy"));

            HealthCheckResult response = await _azureWebJobHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Healthy));
        }

        [Test]
        public async Task WhenAzureWebJobStatusIsNotRunning_ThenAzureWebJobsIsUnhealthy()
        {
            A.CallTo(() => _fakeAzureWebJobHealthCheckService.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Azure webjob is unhealthy", new Exception("Azure webjob is unhealthy")));

            HealthCheckResult response = await _azureWebJobHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }
    }
}
