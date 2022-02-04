﻿using System.Text.Json;
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

            // Send a message to the queue
            await queueClient.SendMessageAsync(JsonSerializer.Serialize(subscriptionRequestMessage));
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Added message in Queue for message:{subscriptionRequestMessage} and _X-Correlation-ID:{CorrelationId}", subscriptionRequestMessage, correlationId);
        }
    }
}
