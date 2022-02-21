using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ISubscriptionServiceData
    {
        Task<string> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken);
    }
}
