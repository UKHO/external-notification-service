using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.SubscriptionService.Configuration
{
    [ExcludeFromCodeCoverage]
    public class D365CallbackConfiguration
    {
        public string D365Uri { get; set; }

        public string D365ApiUri { get; set; }

        public int SucceededStatusCode { get; set; }

        public int FailedStatusCode { get; set; }        

        public int TimeOutInMins { get; set; }

        public int RetryCount { get; set; }

        public int SleepDuration { get; set; }
    }
}
