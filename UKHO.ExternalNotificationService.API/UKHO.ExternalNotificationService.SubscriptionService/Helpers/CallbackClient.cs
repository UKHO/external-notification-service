
using System.Net.Http;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public class CallbackClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CallbackClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
    }
}
