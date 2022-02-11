using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.Common.Helper;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public class AzureMessageQueueHelper : IAzureMessageQueueHelper
    {
        private readonly ILogger<AzureMessageQueueHelper> _logger;

        public AzureMessageQueueHelper(ILogger<AzureMessageQueueHelper> logger)
        {
            _logger = logger;
        }
        public async Task AddQueueMessage<SubscriptionRequestMessage>(string storageConnectionString, string queueName, SubscriptionRequestMessage subscriptionRequestMessage, string correlationId)
        {
            QueueClientOptions queueClientOptions = new()
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            
            QueueClient queueClient = new(storageConnectionString, queueName, queueClientOptions);

            string subscriptionMessageString = JsonSerializer.Serialize(subscriptionRequestMessage);
            // Send a message to the queue
            await queueClient.SendMessageAsync(subscriptionMessageString);           
            
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Added message in Queue for message:{subscriptionMessageString} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscriptionMessageString, subscriptionRequestMessage , correlationId);
        }
    }
}
