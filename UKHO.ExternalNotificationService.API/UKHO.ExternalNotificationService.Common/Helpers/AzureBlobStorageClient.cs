﻿

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobStorageClient : IAzureBlobStorageClient
    {
        public async Task<HealthCheckResult> CheckBlobContainerHealth(string storageAccountConnectionString, string containerName)
        {
            BlobContainerClient container = new BlobContainerClient(storageAccountConnectionString, containerName);
            var blobContainerExists = await container.ExistsAsync();
            if (blobContainerExists)
                return HealthCheckResult.Healthy("Azure blob storage is healthy");
            else
                return HealthCheckResult.Unhealthy("Azure blob storage is unhealthy", new Exception("Azure blob storage connection failed or not available"));
        }
    }
}
