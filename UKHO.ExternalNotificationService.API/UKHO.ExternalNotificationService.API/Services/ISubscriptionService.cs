using UKHO.ExternalNotificationService.API.Models;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface ISubscriptionService
    {
        SubscriptionRequestMessage GetSubscriptionRequestMessage(SubscriptionRequest subscriptionRequest);
        string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null);
    }
}
