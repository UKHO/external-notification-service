namespace UKHO.ExternalNotificationService.API.Models
{
    public class SubscriptionRequest
    {
        public string Id { get; set; }

        public string NotificationType { get; set; }

        public string WebhookUrl { get; set; }

        public bool IsActive { get; set; }

        public string CorrelationId { get; set; }
    }
}
