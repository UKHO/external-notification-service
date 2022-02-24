using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public interface ICallbackClient
    {        
        Task<HttpResponseMessage> GetCallbackD365Client(string externalEntityPath, string accessToken, object externalNotificationEntity, string correlationId, CancellationToken cancellationToken);
    }
}
