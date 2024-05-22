namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class SubscriptionRequest : BaseSubscriptionRequest
    {
        public bool IsActive { get; set; }

        public string D365CorrelationId { get; set; } = string.Empty;
    }
}
