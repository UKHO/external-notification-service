
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
    [TestFixture]
    public class EventHubLoggingHealthCheckTest
    {
        private IEventHubLoggingHealthHelper _fakeEventHubLoggingHealthHelper;
        private EventHubLoggingHealthCheck _eventHubLoggingHealthCheck;
        private ILogger<EventHubLoggingHealthCheck> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<EventHubLoggingHealthCheck>>();
            _fakeEventHubLoggingHealthHelper = A.Fake<IEventHubLoggingHealthHelper>();

            _eventHubLoggingHealthCheck = new EventHubLoggingHealthCheck(_fakeEventHubLoggingHealthHelper, _fakeLogger);
        }

        [Test]
        public async Task WhenEventHubLoggingIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeEventHubLoggingHealthHelper.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            var response = await _eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenEventHubLoggingIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeEventHubLoggingHealthHelper.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Event hub is unhealthy", new Exception("Event hub is unhealthy")));

            var response = await _eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
