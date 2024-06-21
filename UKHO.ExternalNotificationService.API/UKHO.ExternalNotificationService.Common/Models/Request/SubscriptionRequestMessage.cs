namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class SubscriptionRequestMessage : BaseSubscriptionRequest
    {
        public string NotificationTypeTopicName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string D365CorrelationId { get; set; } = string.Empty;

        public string CorrelationId { get; set; } = string.Empty;
    }
}
