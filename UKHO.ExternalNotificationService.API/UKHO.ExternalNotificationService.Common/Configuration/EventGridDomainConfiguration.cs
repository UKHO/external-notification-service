using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventGridDomainConfiguration
    {
        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public string EventGridDomainName { get; set; }        
    }
}
