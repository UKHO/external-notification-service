using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class EventHubLoggingHealthCheck : IHealthCheck
    {
        private readonly IEventHubLoggingHealthHelper _eventHubLoggingHealthHelper;
        private readonly ILogger<EventHubLoggingHealthCheck> _logger;

        public EventHubLoggingHealthCheck(IEventHubLoggingHealthHelper eventHubLoggingHealthClient, ILogger<EventHubLoggingHealthCheck> logger)
        {
            _eventHubLoggingHealthHelper = eventHubLoggingHealthClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _eventHubLoggingHealthHelper.CheckHealthAsync(context,cancellationToken);
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
