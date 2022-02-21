using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureMessageQueueHelper
    {
        Task AddQueueMessage<SubscriptionRequestMessage>(SubscriptionStorageConfiguration ensStorageConfiguration, SubscriptionRequestMessage subscriptionRequestMessage, string correlationId);
        Task<HealthCheckResult> CheckMessageQueueHealth(string storageAccountConnectionString, string queueName);
    }
}
