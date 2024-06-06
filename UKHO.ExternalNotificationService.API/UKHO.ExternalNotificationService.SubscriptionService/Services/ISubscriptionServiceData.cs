using Azure.ResourceManager.EventGrid;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    /// <summary>
    /// The new Azure SDK for EventGrid required the introduction of Azure.ResourceManager.EventGrid.
    /// Subsequently, the method signature has changed to return a DomainTopicEventSubscriptionResource.
    /// </summary>
    public interface ISubscriptionServiceData
    {
        Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken = default);
        
        Task DeleteSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken);
    }
}
