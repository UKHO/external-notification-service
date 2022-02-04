using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helper
{
    public interface IAzureMessageQueueHelper
    {
        Task AddQueueMessage<SubscriptionRequestMessage>(string storageConnectionString, string queueName, SubscriptionRequestMessage subscriptionRequestMessage, string correlationId);
    }
}
