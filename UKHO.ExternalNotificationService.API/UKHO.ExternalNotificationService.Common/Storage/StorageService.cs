
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IOptions<SubscriptionStorageConfiguration> _storageConfig;
        public StorageService(IOptions<SubscriptionStorageConfiguration> storageConfig)
        {
            _storageConfig = storageConfig;
        }
        public string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null)
        {
            string storageAccountAccessKeyValue = !string.IsNullOrEmpty(storageAccountKey) ? storageAccountKey : _storageConfig.Value.StorageAccountKey;
            string storageAccountNameValue = !string.IsNullOrEmpty(storageAccountName) ? storageAccountName : _storageConfig.Value.StorageAccountName;

            if (string.IsNullOrWhiteSpace(storageAccountAccessKeyValue))
            {
                throw new KeyNotFoundException($"Storage account accesskey not found");
            }

            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountNameValue};AccountKey={storageAccountAccessKeyValue};EndpointSuffix=core.windows.net";

            return storageAccountConnectionString;
        }
    }
}
