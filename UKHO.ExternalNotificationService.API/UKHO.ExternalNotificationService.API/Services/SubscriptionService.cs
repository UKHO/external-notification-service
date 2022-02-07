using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        
        public SubscriptionRequestMessage GetSubscriptionRequestMessage(SubscriptionRequest subscriptionRequest)
        {
            var subscriptionRequestMessage = new SubscriptionRequestMessage
            {
                SubscriptionId = subscriptionRequest.Id,
                NotificationType = subscriptionRequest.NotificationType,
                NotificationTypeTopicName = "acc",
                IsActive = subscriptionRequest.IsActive,
                WebhookUrl = subscriptionRequest.WebhookUrl,
                CorrelationId = subscriptionRequest.CorrelationId
            };
            return subscriptionRequestMessage;
        }     
    }
}
