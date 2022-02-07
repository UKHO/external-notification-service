using System.Collections.Generic;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Storage
{
    public class SubscriptionStorageService : ISubscriptionStorageService
    {
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;

        public SubscriptionStorageService(IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration) => _ensStorageConfiguration = ensStorageConfiguration;

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
