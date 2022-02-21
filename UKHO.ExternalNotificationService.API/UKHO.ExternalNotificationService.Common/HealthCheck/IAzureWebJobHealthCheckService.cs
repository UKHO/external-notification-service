
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IAzureWebJobHealthCheckService
    {
        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}
