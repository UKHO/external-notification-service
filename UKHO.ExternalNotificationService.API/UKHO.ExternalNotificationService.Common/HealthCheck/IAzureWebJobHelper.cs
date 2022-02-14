
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IAzureWebJobHelper
    {
        public Task<HealthCheckResult> CheckWebJobsHealth(WebJobDetails webJob);
    }
}
