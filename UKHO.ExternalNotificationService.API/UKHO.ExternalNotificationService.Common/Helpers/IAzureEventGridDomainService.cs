using Azure.Messaging;
using Microsoft.Azure.Management.EventGrid.Models;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureEventGridDomainService
    {
        Task<EventSubscription> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken);

        Task PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default);

        T JsonDeserialize<T>(object data);
    }
}
