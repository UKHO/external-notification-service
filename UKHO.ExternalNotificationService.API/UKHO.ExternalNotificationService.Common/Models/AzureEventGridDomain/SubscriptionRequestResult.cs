using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain
{
    public class SubscriptionRequestResult: BaseSubscriptionRequest
    {
        public string SubscriptionId { get; set; }
        public string ProvisioningState { get; set; }
        public string NotificationType { get; set; }
        public string WebhookUrl { get; set; }
        public string ErrorMessage { get; set; }

        public SubscriptionRequestResult(SubscriptionRequestMessage subscriptionMessage)
        {
            SubscriptionId = subscriptionMessage.SubscriptionId;
            NotificationType = subscriptionMessage.NotificationType;
            WebhookUrl = subscriptionMessage.WebhookUrl;
        }
    }
}
