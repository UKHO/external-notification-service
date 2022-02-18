using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureMessageQueueHelper
    {
        Task AddQueueMessage(SubscriptionStorageConfiguration ensStorageConfiguration, SubscriptionRequestMessage subscriptionRequestMessage);
    }
}
