using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public interface IAuthTokenProvider
    {
        Task<string> GetADAccessToken(SubscriptionRequestMessage subscriptionMessage);
    }
}
