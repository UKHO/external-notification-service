using Azure.ResourceManager.EventGrid;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ISubscriptionServiceData
    {
        Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken = default);
        
        Task DeleteSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken);
    }
}
