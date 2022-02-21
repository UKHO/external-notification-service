using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureEventGridDomainService
    {
        Task<string> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken);
    }
}
