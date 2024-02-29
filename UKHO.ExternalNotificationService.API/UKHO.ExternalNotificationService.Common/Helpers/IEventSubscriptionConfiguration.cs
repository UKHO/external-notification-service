using Azure.ResourceManager.EventGrid;
//using Microsoft.Azure.Management.EventGrid.Models;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IEventSubscriptionConfiguration
    {
        EventGridSubscriptionData SetEventSubscription(SubscriptionRequestMessage subscriptionRequestMessage);
    }
}
