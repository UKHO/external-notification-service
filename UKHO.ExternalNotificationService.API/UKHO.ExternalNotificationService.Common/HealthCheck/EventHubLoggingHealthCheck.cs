using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class EventHubLoggingHealthCheck : IHealthCheck
    {
        private readonly IEventHubLoggingHealthClient _eventHubLoggingHealthClient;
        private readonly ILogger<EventHubLoggingHealthCheck> _logger;

        public EventHubLoggingHealthCheck(IEventHubLoggingHealthClient eventHubLoggingHealthClient, ILogger<EventHubLoggingHealthCheck> logger)
        {
            _eventHubLoggingHealthClient = eventHubLoggingHealthClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _eventHubLoggingHealthClient.CheckHealthAsync(context, cancellationToken);
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                _logger.LogDebug(EventIds.EventHubLoggingIsHealthy.ToEventId(), "Event hub is healthy");
            }
            else
            {
                _logger.LogError(EventIds.EventHubLoggingIsUnhealthy.ToEventId(), healthCheckResult.Exception, "Event hub is unhealthy responded with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
