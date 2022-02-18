using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventGrid.Models;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ISubscriptionServiceData
    {
        Task<EventSubscription> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken);
    }
}
