using Azure.Messaging;
//using Microsoft.Azure.Management.EventGrid.Models;
using Azure.ResourceManager.EventGrid;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
#nullable enable  //rhz will try to make this global later
    public interface IAzureEventGridDomainService
    {
        Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken);
        Task DeleteSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken);

        Task PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default);

        T? ConvertObjectTo<T>(object data) where T : class;
    }
}
