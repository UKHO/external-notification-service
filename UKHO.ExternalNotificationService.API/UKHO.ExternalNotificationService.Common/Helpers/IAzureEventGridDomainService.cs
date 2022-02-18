using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventGrid.Models;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureEventGridDomainService
    {
        Task<EventSubscription> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken);
    }
}
