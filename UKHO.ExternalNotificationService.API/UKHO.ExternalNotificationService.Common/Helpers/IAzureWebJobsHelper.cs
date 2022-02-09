
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureWebJobsHelper
    {
        public Task<HealthCheckResult> CheckWebJobsHealth(WebJobDetails webJob);
    }
}
