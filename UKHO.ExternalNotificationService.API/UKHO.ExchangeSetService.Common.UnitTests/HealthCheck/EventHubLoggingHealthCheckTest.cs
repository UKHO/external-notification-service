
using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.API.UnitTests.HealthCheck
{
    public class EventHubLoggingHealthCheckTest
    {
        private IEventHubLoggingHealthClient fakeEventHubLoggingHealthClient;
        private EventHubLoggingHealthCheck eventHubLoggingHealthCheck;
        private ILogger<EventHubLoggingHealthCheck> fakeLogger;

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<EventHubLoggingHealthCheck>>();
            fakeEventHubLoggingHealthClient = A.Fake<IEventHubLoggingHealthClient>();

            eventHubLoggingHealthCheck = new EventHubLoggingHealthCheck(fakeEventHubLoggingHealthClient, fakeLogger);
        }

        [Test]
        public async Task WhenEventHubLoggingIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            var response = await eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenEventHubLoggingIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Event hub is unhealthy", new Exception("Event hub is unhealthy")));

            var response = await eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
