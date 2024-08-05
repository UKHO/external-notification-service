namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class BaseSubscriptionRequest
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
    }
}
