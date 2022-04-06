using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public class HandleDeadLetterService : IHandleDeadLetterService
    {
        private readonly ICallbackService _callbackService;
        private readonly IAzureMessageQueueHelper _azureMessageQueueHelper;
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;
        private readonly ILogger<HandleDeadLetterService> _logger;
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;

        public HandleDeadLetterService(ICallbackService callbackService,
                                       IAzureMessageQueueHelper azureMessageQueueHelper,
                                       IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration,
                                       ILogger<HandleDeadLetterService> logger,
                                       IOptions<D365CallbackConfiguration> d365CallbackConfiguration)
        {
            _callbackService = callbackService;
            _azureMessageQueueHelper = azureMessageQueueHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
            _logger = logger;
            _d365CallbackConfiguration = d365CallbackConfiguration;
        }

        public async Task ProcessDeadLetter(string filePath, string subscriptionId, SubscriptionRequestMessage subscriptionRequestMessage)
        {
            ExternalNotificationEntityWithStateCode externalNotificationEntityWithStateCode = new()
            {
                ResponseStatusCode = _d365CallbackConfiguration.Value.FailedStatusCode,
                ResponseDetails = $"Failed to deliver notification therefore subscription marked as inactive @Time: {DateTime.UtcNow}.",
                ResponseStateCode = _d365CallbackConfiguration.Value.InactiveStateCode
            };                                                       
            string entityPath = $"ukho_externalnotifications({subscriptionId})";

            _logger.LogInformation(EventIds.DeadLetterCallbackToD365Started.ToEventId(),
            "DeadLetter process send request callback to D365 using Dataverse start with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{subscriptionId} and _D365-Correlation-ID:{correlationId} , _X-Correlation-ID:{CorrelationId}", externalNotificationEntityWithStateCode.ResponseStatusCode, externalNotificationEntityWithStateCode.ResponseDetails, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _callbackService.DeadLetterCallbackToD365UsingDataverse(entityPath, externalNotificationEntityWithStateCode, subscriptionRequestMessage);
        }

        public async Task<DateTime> GetBlockBlobLastModifiedDate(string filePath)
        {
            return await _azureMessageQueueHelper.GetBlockBlobLastModifiedDate(_ensStorageConfiguration.Value, filePath);
        }
    }
}
