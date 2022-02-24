
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IAzureBlobStorageHelper
    {
        Task<HealthCheckResult> CheckBlobContainerHealth(string storageAccountConnectionString, string containerName);
    }
}
