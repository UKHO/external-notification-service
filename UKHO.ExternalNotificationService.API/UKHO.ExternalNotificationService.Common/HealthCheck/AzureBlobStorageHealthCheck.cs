
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Storage;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class AzureBlobStorageHealthCheck : IHealthCheck
    {
        private readonly IAzureBlobStorageHelper _azureBlobStorageHelper;
        private readonly IStorageService _storageService;
        private readonly IOptions<SubscriptionStorageConfiguration> _subscriptionStorageConfiguration;
        private readonly ILogger<AzureBlobStorageHealthCheck> _logger;

        public AzureBlobStorageHealthCheck(IAzureBlobStorageHelper azureBlobStorageHelper,
                                           IStorageService storageService,
                                           IOptions<SubscriptionStorageConfiguration> subscriptionStorageConfiguration,
                                           ILogger<AzureBlobStorageHealthCheck> logger)
        {
            _azureBlobStorageHelper = azureBlobStorageHelper;
            _storageService = storageService;
            _subscriptionStorageConfiguration = subscriptionStorageConfiguration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                string storageAccountConnectionString = _storageService.GetStorageAccountConnectionString(_subscriptionStorageConfiguration.Value.StorageAccountName, _subscriptionStorageConfiguration.Value.StorageAccountKey);
                HealthCheckResult azureBlobStorageHealthStatus = await _azureBlobStorageHelper.CheckBlobContainerHealth(storageAccountConnectionString, _subscriptionStorageConfiguration.Value.StorageContainerName);

                if (azureBlobStorageHealthStatus.Status == HealthStatus.Unhealthy)
                {
                    _logger.LogError(EventIds.AzureBlobStorageIsUnhealthy.ToEventId(), azureBlobStorageHealthStatus.Exception, "Azure blob storage is unhealthy with error {Message}", azureBlobStorageHealthStatus.Exception.Message);
                    azureBlobStorageHealthStatus = HealthCheckResult.Unhealthy("Azure blob storage is unhealthy", azureBlobStorageHealthStatus.Exception);
                    return azureBlobStorageHealthStatus;
                }
                _logger.LogDebug(EventIds.AzureBlobStorageIsHealthy.ToEventId(), "Azure blob storage is healthy");
                return azureBlobStorageHealthStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.AzureBlobStorageIsUnhealthy.ToEventId(), ex, "Azure blob storage is unhealthy with error {Message}", ex.Message);
                return HealthCheckResult.Unhealthy("Azure blob storage is unhealthy", ex);
            }
        }
    }
}
