using Azure.ResourceManager.EventGrid;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IEventSubscriptionConfiguration
    {
        EventGridSubscriptionData SetEventSubscription(SubscriptionRequestMessage subscriptionRequestMessage);
    }
}
