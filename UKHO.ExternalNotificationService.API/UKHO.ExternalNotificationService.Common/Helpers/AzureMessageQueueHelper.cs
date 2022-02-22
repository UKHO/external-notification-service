using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
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
    }
}
