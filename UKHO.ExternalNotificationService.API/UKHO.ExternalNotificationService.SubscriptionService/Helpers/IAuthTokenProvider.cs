
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public interface IAuthTokenProvider
    {
        Task<string> GetADAccessToken();
    }
}
