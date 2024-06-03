using Azure.Core;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.EventGrid.Models;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public class EventSubscriptionConfiguration : IEventSubscriptionConfiguration
    {
        private readonly EventGridDomainConfiguration _eventGridDomainConfig;
        private readonly SubscriptionStorageConfiguration _subscriptionStorageConfiguration;

        public EventSubscriptionConfiguration(IOptions<EventGridDomainConfiguration> eventGridDomainConfig,
            IOptions<SubscriptionStorageConfiguration> subscriptionStorageConfiguration)
        {
            _eventGridDomainConfig = eventGridDomainConfig.Value;
            _subscriptionStorageConfiguration = subscriptionStorageConfiguration.Value;
        }

        /// <summary>
        /// Sets the event subscription based on the provided subscription request message.
        /// Change of return type due to the change in the EventGrid SDK.
        /// </summary>
        /// <param name="subscriptionRequestMessage">The subscription request message.</param>
        /// <returns>The event grid subscription data.</returns>
        public EventGridSubscriptionData SetEventSubscription(SubscriptionRequestMessage subscriptionRequestMessage)
        {
            return new()
            {
                Destination = SetWebHookEventSubscriptionDestination(subscriptionRequestMessage.WebhookUrl),
                EventDeliverySchema = SetEventDeliverySchema,
                RetryPolicy = SetRetryPolicy(),
                DeadLetterDestination = SetStorageBlobDeadLetterDestination()
            };
        }

        /// <summary>
        /// Sets the web hook event subscription destination.
        /// Change due to the change in the EventGrid SDK.
        /// </summary>
        /// <param name="webhookUrl">The webhook URL.</param>
        /// <returns>The web hook event subscription destination.</returns>
        private static WebHookEventSubscriptionDestination SetWebHookEventSubscriptionDestination(string webhookUrl) => new()
        {
            Endpoint = new(webhookUrl)
        };

        /// <summary>
        /// Gets the event delivery schema.
        /// Change due to the change in the EventGrid SDK.
        /// </summary>
        private static string SetEventDeliverySchema => EventDeliverySchema.CloudEventSchemaV1_0.ToString();

        /* Retry policy decides when an event can be marked as expired. 
           The default retry policy keeps the event alive for 24 hrs (=1440 mins or 30 retries with exponential backoffs)
           An event is marked as expired once any of the retry policy limits are exceeded. 
           Note: The below configuration for the retry policy will cause events to expire after one delivery attempt. 
           This is only to make it easier to help test/verify dead letter destinations quickly.
        */
        /// <summary>
        /// Sets the retry policy for event subscription.
        /// Change due to the change in the EventGrid SDK.
        /// </summary>
        /// <returns>The event subscription retry policy.</returns>
        private EventSubscriptionRetryPolicy SetRetryPolicy() => new()
        {
            MaxDeliveryAttempts = _eventGridDomainConfig.MaxDeliveryAttempts,
            EventTimeToLiveInMinutes = _eventGridDomainConfig.EventTimeToLiveInMinutes,
        };

        // With dead-letter destination configured, all expired events will be delivered to this destination.
        // Note: only Storage Blobs are supported as dead letter destinations as of now.
        /// <summary>
        /// Sets the storage blob dead letter destination for expired events.
        /// Changes due to the change in the EventGrid SDK.
        /// </summary>
        /// <returns>The storage blob dead letter destination.</returns>
        private StorageBlobDeadLetterDestination SetStorageBlobDeadLetterDestination()
        {
            string deadLetterDestinationResourceId = $"/subscriptions/{_eventGridDomainConfig.SubscriptionId}/resourceGroups/{_eventGridDomainConfig.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{_subscriptionStorageConfiguration.StorageAccountName}";
            return new StorageBlobDeadLetterDestination()
            {

                ResourceId = new(deadLetterDestinationResourceId),
                BlobContainerName = _subscriptionStorageConfiguration.StorageContainerName,
            };
        }
    }
}
