﻿using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventGridDomainConfiguration
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string EventGridDomainName { get; set; }        
        public int MaxDeliveryAttempts { get; set; }
        public int EventTimeToLiveInMinutes { get; set; }
        public string EventGridDomainAccessKey { get; set; }
        public string EventGridDomainEndpoint { get; set; }
    }
}
