using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface ISubscriptionService
    {
        SubscriptionRequestMessage GetSubscriptionRequestMessage(SubscriptionRequest subscriptionRequest);
        
    }
}
