using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface ICallbackService
    {
        Task<HttpResponseMessage> CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, SubscriptionRequestMessage subscriptionMessage);

        Task<HttpResponseMessage> DeadLetterCallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntityStateCode, SubscriptionRequestMessage subscriptionMessage);
    }
}
