
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class AzureWebJobHealthCheck : IHealthCheck
    {
        private readonly IAzureWebJobHealthCheckService _azureWebJobHealthCheckService;
        private readonly ILogger<AzureWebJobHealthCheck> _logger;

        public AzureWebJobHealthCheck(IAzureWebJobHealthCheckService azureWebJobHealthCheckService,
                                       ILogger<AzureWebJobHealthCheck> logger)
        {
            _azureWebJobHealthCheckService = azureWebJobHealthCheckService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckResult = await _azureWebJobHealthCheckService.CheckHealthAsync();
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                _logger.LogDebug(EventIds.AzureWebJobIsHealthy.ToEventId(), "Azure webjob is healthy");
            }
            else
            {
                _logger.LogError(EventIds.AzureWebJobIsUnhealthy.ToEventId(), healthCheckResult.Exception, "Azure webjob is unhealthy with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
