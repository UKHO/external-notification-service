
namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class SubscriptionRequest
    {
        public string SubscriptionId { get; set; }

        public string NotificationType { get; set; }

        public string WebhookUrl { get; set; }

        public bool IsActive { get; set; }

        public string CorrelationId { get; set; }
    }
}
