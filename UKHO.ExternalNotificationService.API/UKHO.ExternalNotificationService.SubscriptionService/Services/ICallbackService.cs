
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ICallbackService
    {
        Task CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, string D365CorrelationId, string CorrelationId);
    }
}
