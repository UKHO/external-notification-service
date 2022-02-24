using Microsoft.Azure.Management.EventGrid.Models;
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

        public EventSubscription SetEventSubscription(SubscriptionRequestMessage subscriptionRequestMessage)
        {
            return new()
            {
                Destination = SetWebHookEventSubscriptionDestination(subscriptionRequestMessage.WebhookUrl),
                EventDeliverySchema = SetEventDeliverySchema,
                RetryPolicy = SetRetryPolicy(),
                DeadLetterDestination = SetStorageBlobDeadLetterDestination()
            };
        }

        private static WebHookEventSubscriptionDestination SetWebHookEventSubscriptionDestination(string webhookUrl) => new()
        {
            EndpointUrl = webhookUrl
        };

        private static string SetEventDeliverySchema => EventDeliverySchema.CloudEventSchemaV10;

        /* Retry policy decides when an event can be marked as expired. 
           The default retry policy keeps the event alive for 24 hrs (=1440 mins or 30 retries with exponential backoffs)
           An event is marked as expired once any of the retry policy limits are exceeded. 
           Note: The below configuration for the retry policy will cause events to expire after one delivery attempt. 
           This is only to make it easier to help test/verify dead letter destinations quickly.
        */
        private RetryPolicy SetRetryPolicy() => new()
        {
            MaxDeliveryAttempts = _eventGridDomainConfig.MaxDeliveryAttempts,
            EventTimeToLiveInMinutes = _eventGridDomainConfig.EventTimeToLiveInMinutes,
        };

        // With dead-letter destination configured, all expired events will be delivered to this destination.
        // Note: only Storage Blobs are supported as dead letter destinations as of now.
        private StorageBlobDeadLetterDestination SetStorageBlobDeadLetterDestination()
        {
            string deadLetterDestinationResourceId = $"/subscriptions/{_eventGridDomainConfig.SubscriptionId}/resourceGroups/{_eventGridDomainConfig.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{_subscriptionStorageConfiguration.StorageAccountName}";
            return new StorageBlobDeadLetterDestination()
            {
                ResourceId = deadLetterDestinationResourceId,
                BlobContainerName = _subscriptionStorageConfiguration.StorageContainerName,
            };
        }
    }
}
