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
        private IEventHubLoggingHealthClient _fakeEventHubLoggingHealthClient;
        private EventHubLoggingHealthCheck _eventHubLoggingHealthCheck;
        private ILogger<EventHubLoggingHealthCheck> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<EventHubLoggingHealthCheck>>();
            _fakeEventHubLoggingHealthClient = A.Fake<IEventHubLoggingHealthClient>();

            _eventHubLoggingHealthCheck = new EventHubLoggingHealthCheck(_fakeEventHubLoggingHealthClient, _fakeLogger);
        }

        [Test]
        public async Task WhenEventHubLoggingIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await _eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Healthy));
        }

        [Test]
        public async Task WhenEventHubLoggingIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Event hub is unhealthy", new Exception("Event hub is unhealthy")));

            HealthCheckResult response = await _eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(response.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }
    }
}
