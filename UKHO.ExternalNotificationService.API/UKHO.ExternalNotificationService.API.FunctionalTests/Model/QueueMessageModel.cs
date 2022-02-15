
namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class QueueMessageModel
    {
        public string SubscriptionId { get; set; }
        public string NotificationType { get; set; }
        public string NotificationTypeTopicName { get; set; }
        public string WebhookUrl { get; set; }
        public bool IsActive { get; set; }
        public string D365CorrelationId { get; set; }
        public string CorrelationId { get; set; }
    }
}
