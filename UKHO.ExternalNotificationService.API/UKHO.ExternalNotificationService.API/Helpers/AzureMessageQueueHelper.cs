using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using UKHO.ExternalNotificationService.API.Helper;

namespace UKHO.ExternalNotificationService.API.Helpers
{
    public class AzureMessageQueueHelper : IAzureMessageQueueHelper
    {
        public async Task AddQueueMessage<T>(string storageConnectionString, string queueName, T message)
        {
            QueueClientOptions queueClientOptions = new QueueClientOptions()
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            QueueClient queueClient = new QueueClient(storageConnectionString, queueName, queueClientOptions);

            // Send a message to the queue
            await queueClient.SendMessageAsync(JsonSerializer.Serialize(message));
        }
    }
}
