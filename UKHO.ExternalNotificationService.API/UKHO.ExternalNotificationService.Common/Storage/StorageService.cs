
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IOptions<EnsFulfilmentStorageConfiguration> _storageConfig;
        public StorageService(IOptions<EnsFulfilmentStorageConfiguration> storageConfig)
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
