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
            _logger.LogInformation(EventIds.ENSSubscriptionMarkedAsInactiveStart.ToEventId(),
                      "Process to mark subscription as inactive started for SubscriptionId:{SubscriptionId}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            ExternalNotificationEntityWithStateCode externalNotificationEntityWithStateCode = new()
            {
                ResponseStatusCode = _d365CallbackConfiguration.Value.FailedStatusCode,
                ResponseDetails = $"Failed to deliver notification therefore subscription marked as inactive @Time: {DateTime.UtcNow}.",
                ResponseStateCode = _d365CallbackConfiguration.Value.InactiveStateCode
            };                                                       
            string entityPath = $"ukho_externalnotifications({subscriptionId})";

            _logger.LogInformation(EventIds.CallbackToD365ForDeadLetterProcessingStarted.ToEventId(),
            "Callback to D365 started for dead letter processing to mark subscription as inactive with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{subscriptionId} and _D365-Correlation-ID:{correlationId} , _X-Correlation-ID:{CorrelationId}", externalNotificationEntityWithStateCode.ResponseStatusCode, externalNotificationEntityWithStateCode.ResponseDetails, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _callbackService.DeadLetterCallbackToD365UsingDataverse(entityPath, externalNotificationEntityWithStateCode, subscriptionRequestMessage);

            _logger.LogInformation(EventIds.ENSSubscriptionMarkedAsInactiveCompleted.ToEventId(),
                 "Process to mark subscription as inactive completed for SubscriptionId:{SubscriptionId}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            _logger.LogInformation(EventIds.ENSMoveDeadLetterContainerBlobStarted.ToEventId(),
                 "Process to move dead letter container blob to destination container blob started for SubscriptionId:{SubscriptionId}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _azureMessageQueueHelper.DeadLetterMoveBlob(_ensStorageConfiguration.Value, filePath);

            _logger.LogInformation(EventIds.ENSMoveDeadLetterContainerBlobCompleted.ToEventId(),
                 "Process to move dead letter container blob to destination container blob completed for SubscriptionId:{SubscriptionId}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);
        }
    }
}
