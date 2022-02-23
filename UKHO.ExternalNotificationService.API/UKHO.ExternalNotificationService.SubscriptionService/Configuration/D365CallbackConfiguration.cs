

namespace UKHO.ExternalNotificationService.SubscriptionService.Configuration
{
    public class D365CallbackConfiguration
    {
        public string MicrosoftOnlineLoginUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TenantId { get; set; }

        public string D365Uri { get; set; }

        public string D365ApiUri { get; set; }

        public int SucceededStatusCode { get; set; }

        public int TimeOutInMins { get; set; }
    }
}
