
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureBlobStorageHelper
    {
        Task<HealthCheckResult> CheckBlobContainerHealth(string storageAccountConnectionString, string containerName);
    }
}
