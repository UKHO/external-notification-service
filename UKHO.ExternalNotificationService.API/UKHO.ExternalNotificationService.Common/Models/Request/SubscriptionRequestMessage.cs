﻿namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class SubscriptionRequestMessage : BaseSubscriptionRequest
    {
        public string NotificationTypeTopicName { get; set; }

        public bool IsActive { get; set; }

        public string D365CorrelationId { get; set; }

        public string CorrelationId { get; set; }
    }
}
