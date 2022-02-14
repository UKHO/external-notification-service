using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
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
        public async Task AddQueueMessage<SubscriptionRequestMessage>(SubscriptionStorageConfiguration ensStorageConfiguration , SubscriptionRequestMessage subscriptionRequestMessage, string correlationId)
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
            
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Added message in Queue for message:{subscriptionMessageString} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscriptionMessageString, subscriptionRequestMessage , correlationId);
        }
    }
}
