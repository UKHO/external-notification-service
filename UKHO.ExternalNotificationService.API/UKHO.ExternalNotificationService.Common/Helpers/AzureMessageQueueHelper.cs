
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using UKHO.ExternalNotificationService.Common.Logging;

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

        public async Task<HealthCheckResult> CheckMessageQueueHealth(string storageAccountConnectionString, string queueName)
        { 
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            bool isQueueMessageExists = await queue.ExistsAsync();
            if (isQueueMessageExists)
                return HealthCheckResult.Healthy("Azure message queue is healthy");
            else
                return HealthCheckResult.Unhealthy("Azure message queue is unhealthy", new Exception($"Azure message queue {queueName} does not exists"));
        }

        public async Task AddQueueMessage<SubscriptionRequestMessage>(string storageConnectionString, string queueName, SubscriptionRequestMessage subscriptionRequestMessage, string correlationId)
        {
            QueueClientOptions queueClientOptions = new()
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            QueueClient queueClient = new(storageConnectionString, queueName, queueClientOptions);

            // Send a message to the queue
            await queueClient.SendMessageAsync(JsonSerializer.Serialize(subscriptionRequestMessage));
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Added message in Queue for message:{subscriptionRequestMessage} and _X-Correlation-ID:{CorrelationId}", subscriptionRequestMessage, correlationId);
        }
    }
}
