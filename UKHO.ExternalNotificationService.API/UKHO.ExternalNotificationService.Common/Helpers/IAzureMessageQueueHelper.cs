using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helper
{
    public interface IAzureMessageQueueHelper
    {
        Task AddQueueMessage<SubscriptionRequestMessage>(SubscriptionStorageConfiguration ensStorageConfiguration, SubscriptionRequestMessage subscriptionRequestMessage, string correlationId);
    }
}
