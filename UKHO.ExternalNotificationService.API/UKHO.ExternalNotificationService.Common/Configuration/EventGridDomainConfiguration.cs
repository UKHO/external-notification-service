namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class EventGridDomainConfiguration
    {
        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public string EventGridDomainName { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageContainerName { get; set; }
        public int MaxDeliveryAttempts { get; set; }
        public int EventTimeToLiveInMinutes { get; set; }
    }
}
