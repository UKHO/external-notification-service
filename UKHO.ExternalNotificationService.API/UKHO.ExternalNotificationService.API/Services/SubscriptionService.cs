
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IOptions<EnsSubscriptionStorageConfiguration> _ensStorageConfiguration;

        public SubscriptionService(IOptions<EnsSubscriptionStorageConfiguration> ensStorageConfiguration)
        {
            _ensStorageConfiguration = ensStorageConfiguration;
        }
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

        public string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null)
        {
            string ensStorageAccountAccessKeyValue = !string.IsNullOrEmpty(storageAccountKey) ? storageAccountKey : _ensStorageConfiguration.Value.StorageAccountKey;
            string ensStorageAccountName = !string.IsNullOrEmpty(storageAccountName) ? storageAccountName : _ensStorageConfiguration.Value.StorageAccountName;

            if (string.IsNullOrWhiteSpace(ensStorageAccountAccessKeyValue))
            {
                throw new KeyNotFoundException($"Storage account accesskey not found");
            }

            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={ensStorageAccountName};AccountKey={ensStorageAccountAccessKeyValue};EndpointSuffix=core.windows.net";

            return storageAccountConnectionString;
        }
    }
}
