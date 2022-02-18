namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class BaseSubscriptionRequest
    {
        public string SubscriptionId { get; set; }
        public string NotificationType { get; set; }
        public string WebhookUrl { get; set; }
    }
}
