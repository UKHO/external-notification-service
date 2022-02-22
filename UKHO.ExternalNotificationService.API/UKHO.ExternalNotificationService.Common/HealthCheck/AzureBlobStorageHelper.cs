
using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobStorageHelper : IAzureBlobStorageHelper
    {
        public async Task<HealthCheckResult> CheckBlobContainerHealth(string storageAccountConnectionString, string containerName)
        {
            BlobContainerClient container = new(storageAccountConnectionString, containerName);
            bool isContainerExists = await container.ExistsAsync();
            if (isContainerExists)
                return HealthCheckResult.Healthy("Azure blob storage is healthy");
            else
                return HealthCheckResult.Unhealthy("Azure blob storage is unhealthy", new Exception("Azure blob storage connection failed or not available"));
        }
    }
}
