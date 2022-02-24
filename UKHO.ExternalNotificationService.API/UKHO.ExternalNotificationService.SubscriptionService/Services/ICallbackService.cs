using System.Net.Http;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ICallbackService
    {
        Task<HttpResponseMessage> CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, string d365CorrelationId, string correlationId);
    }
}
