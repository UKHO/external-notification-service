
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IAzureWebJobHelper
    {
        Task<HealthCheckResult> CheckWebJobsHealth(WebJobDetails webJob);
    }
}
