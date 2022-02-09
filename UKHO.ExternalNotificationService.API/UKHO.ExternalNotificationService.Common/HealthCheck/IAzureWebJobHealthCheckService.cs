
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IAzureWebJobHealthCheckService
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}
