using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventGridDomainConfiguration
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public string ResourceGroup { get; set; } = string.Empty;
        public string EventGridDomainName { get; set; } = string.Empty;                
        public int MaxDeliveryAttempts { get; set; }
        public int EventTimeToLiveInMinutes { get; set; }
        public string EventGridDomainAccessKey { get; set; } = string.Empty;
        public string EventGridDomainEndpoint { get; set; } = string.Empty;
    }
}
