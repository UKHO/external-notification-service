using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public class AzureMoveBlobHelper : IAzureMoveBlobHelper
    {
        private readonly ILogger<AzureMoveBlobHelper> _logger;

        public AzureMoveBlobHelper(ILogger<AzureMoveBlobHelper> logger)
        {
            _logger = logger;
        }

        public async Task DeadLetterMoveBlob(SubscriptionStorageConfiguration ensStorageConfiguration, string path)
        {
            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={ensStorageConfiguration.StorageAccountName};AccountKey={ensStorageConfiguration.StorageAccountKey};EndpointSuffix=core.windows.net";

            CloudStorageAccount sourceStorageConnectionString = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudStorageAccount destinationStorageConnectionString = CloudStorageAccount.Parse(storageAccountConnectionString);

            CloudBlobClient sourceCloudBlobClient = sourceStorageConnectionString.CreateCloudBlobClient();
            CloudBlobClient targetCloudBlobClient = destinationStorageConnectionString.CreateCloudBlobClient();
            CloudBlobContainer sourceContainer = sourceCloudBlobClient.GetContainerReference(ensStorageConfiguration.StorageContainerName);
            CloudBlobContainer destinationContainer = targetCloudBlobClient.GetContainerReference(ensStorageConfiguration.DestinationContainerName);

            string blobName = path;
            await destinationContainer.CreateIfNotExistsAsync();

            CloudBlockBlob sourceBlob = sourceContainer.GetBlockBlobReference(blobName);
            CloudBlockBlob targetBlob = destinationContainer.GetBlockBlobReference(blobName);

            await targetBlob.StartCopyAsync(sourceBlob);

            if (await targetBlob.ExistsAsync())
            {
                await sourceBlob.DeleteIfExistsAsync();
            }
        }

        public async Task<DateTime> GetBlockBlobLastModifiedDate(SubscriptionStorageConfiguration ensStorageConfiguration, string path)
        {
            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={ensStorageConfiguration.StorageAccountName};AccountKey={ensStorageConfiguration.StorageAccountKey};EndpointSuffix=core.windows.net";
            string blobName = path;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ensStorageConfiguration.StorageContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            await blockBlob.FetchAttributesAsync();
            var lastModifiedDate = blockBlob.Properties.LastModified;

            return lastModifiedDate.Value.UtcDateTime;
        }
    }
}