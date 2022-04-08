
using Azure.Storage.Queues;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureMessageQueueHelper : IAzureMessageQueueHelper
    {
      
        private readonly ILogger<AzureMessageQueueHelper> _logger;

        public AzureMessageQueueHelper(ILogger<AzureMessageQueueHelper> logger)
        {
            _logger = logger;
        }
        public async Task AddQueueMessage(SubscriptionStorageConfiguration ensStorageConfiguration , SubscriptionRequestMessage subscriptionRequestMessage)
        {
            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={ensStorageConfiguration.StorageAccountName};AccountKey={ensStorageConfiguration.StorageAccountKey};EndpointSuffix=core.windows.net";
           
            QueueClientOptions queueClientOptions = new()
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            
            QueueClient queueClient = new(storageAccountConnectionString, ensStorageConfiguration.QueueName, queueClientOptions);

            string subscriptionMessageString = JsonSerializer.Serialize(subscriptionRequestMessage);
            // Send a message to the queue
            await queueClient.SendMessageAsync(subscriptionMessageString);           
            
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Added message in Queue for message:{subscriptionMessageString} with SubscriptionId:{SubscriptionId}, _D365-Correlation-ID:{D365correlationId} and _X-Correlation-ID:{correlationId}", subscriptionMessageString, subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId , subscriptionRequestMessage.CorrelationId);
        }

        public async Task<HealthCheckResult> CheckMessageQueueHealth(string storageAccountConnectionString, string queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            bool queueExists = await queue.ExistsAsync();
            if (queueExists)
                return HealthCheckResult.Healthy("Azure message queue is healthy");
            else
                return HealthCheckResult.Unhealthy("Azure message queue is unhealthy", new Exception($"Azure message queue {queueName} does not exists"));
        }

        public async Task CopyDeadLetterBlob(SubscriptionStorageConfiguration ensStorageConfiguration, string path)
        {
            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={ensStorageConfiguration.StorageAccountName};AccountKey={ensStorageConfiguration.StorageAccountKey};EndpointSuffix=core.windows.net";

            CloudStorageAccount sourceStorageConnectionString = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudStorageAccount destinationStorageConnectionString = CloudStorageAccount.Parse(storageAccountConnectionString);

            CloudBlobClient sourceCloudBlobClient = sourceStorageConnectionString.CreateCloudBlobClient();
            CloudBlobClient targetCloudBlobClient = destinationStorageConnectionString.CreateCloudBlobClient();
            CloudBlobContainer sourceContainer = sourceCloudBlobClient.GetContainerReference(ensStorageConfiguration.StorageContainerName);
            CloudBlobContainer destinationContainer = targetCloudBlobClient.GetContainerReference(ensStorageConfiguration.DeadLetterDestinationContainerName);

            await destinationContainer.CreateIfNotExistsAsync();

            CloudBlockBlob sourceBlob = sourceContainer.GetBlockBlobReference(path);
            CloudBlockBlob targetBlob = destinationContainer.GetBlockBlobReference(path);

            await targetBlob.StartCopyAsync(sourceBlob);

            if (await targetBlob.ExistsAsync())
            {
                await sourceBlob.DeleteIfExistsAsync();
            }
        }
    }
}
