using Microsoft.Azure.Management.EventGrid.Models;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IEventSubscriptionConfiguration
    {
        WebHookEventSubscriptionDestination SetWebHookEventSubscriptionDestination(string webhookUrl);
        string SetEventDeliverySchema { get; }
        RetryPolicy SetRetryPolicy();
        StorageBlobDeadLetterDestination SetStorageBlobDeadLetterDestination();
    }
}
