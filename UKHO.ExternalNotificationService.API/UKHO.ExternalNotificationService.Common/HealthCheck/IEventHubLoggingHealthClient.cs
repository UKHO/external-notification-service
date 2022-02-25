
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IEventHubLoggingHealthClient
    {
        Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    }
}
