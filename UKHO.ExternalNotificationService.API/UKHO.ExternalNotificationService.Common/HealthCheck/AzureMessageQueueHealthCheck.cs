
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Storage;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class AzureMessageQueueHealthCheck : IHealthCheck
    {
        private readonly IOptions<EnsFulfilmentStorageConfiguration> _ensFulfilmentStorageConfiguration;
        private readonly ILogger<AzureMessageQueueHelper> _logger;
        private readonly IStorageService _storageService;
        private readonly IAzureMessageQueueHelper _azureMessageQueueHelper;

        public AzureMessageQueueHealthCheck(IOptions<EnsFulfilmentStorageConfiguration> ensFulfilmentStorageConfiguration,
                                            ILogger<AzureMessageQueueHelper> logger,
                                            IStorageService storageService,
                                            IAzureMessageQueueHelper azureMessageQueueHelper)
        {
            _ensFulfilmentStorageConfiguration = ensFulfilmentStorageConfiguration;
            _logger = logger;
            _storageService = storageService;
            _azureMessageQueueHelper = azureMessageQueueHelper;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var messageQueuesHealth = CheckMessageQueuesHealth();
                await Task.WhenAll(messageQueuesHealth);

                if (messageQueuesHealth.Result.Status == HealthStatus.Healthy)
                {
                    _logger.LogDebug(EventIds.AzureMessageQueueIsHealthy.ToEventId(), "Azure message queue is healthy");
                    return HealthCheckResult.Healthy("Azure message queue is healthy");
                }
                else
                {
                    _logger.LogError(EventIds.AzureMessageQueueIsUnhealthy.ToEventId(), messageQueuesHealth.Exception, "Azure message queue is unhealthy with error {Message}", messageQueuesHealth.Result.Exception.Message);
                    return HealthCheckResult.Unhealthy("Azure message queue is unhealthy", messageQueuesHealth.Exception);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.AzureMessageQueueIsUnhealthy.ToEventId(), ex, "Azure message queue is unhealthy with error {Message}", ex.Message);
                return HealthCheckResult.Unhealthy("Azure message queue is unhealthy", ex);
            }
        }

        private async Task<HealthCheckResult> CheckMessageQueuesHealth()
        {
            string storageAccountConnectionString = string.Empty;
            HealthCheckResult messageQueueHealthStatus = new HealthCheckResult(HealthStatus.Healthy, "Azure message queue is healthy");

            storageAccountConnectionString = _storageService.GetStorageAccountConnectionString(_ensFulfilmentStorageConfiguration.Value.StorageAccountName, _ensFulfilmentStorageConfiguration.Value.StorageAccountKey);
            messageQueueHealthStatus = await _azureMessageQueueHelper.CheckMessageQueueHealth(storageAccountConnectionString, _ensFulfilmentStorageConfiguration.Value.QueueName);
            return messageQueueHealthStatus;
        }
    }
}
