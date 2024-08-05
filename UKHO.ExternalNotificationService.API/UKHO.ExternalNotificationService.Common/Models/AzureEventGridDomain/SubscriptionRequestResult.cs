using System.Diagnostics.CodeAnalysis;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain
{
    [ExcludeFromCodeCoverage]
    public class SubscriptionRequestResult: BaseSubscriptionRequest
    {
        public string ProvisioningState { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public SubscriptionRequestResult(SubscriptionRequestMessage subscriptionMessage)
        {
            SubscriptionId = subscriptionMessage.SubscriptionId;
            NotificationType = subscriptionMessage.NotificationType;
            WebhookUrl = subscriptionMessage.WebhookUrl;
        }
    }
}
