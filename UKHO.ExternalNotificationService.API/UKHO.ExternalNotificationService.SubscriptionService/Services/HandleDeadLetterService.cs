using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
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
        private readonly IAzureMoveBlobHelper _azureMoveBlobHelper;
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;
        private readonly ILogger<HandleDeadLetterService> _logger;
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;

        public HandleDeadLetterService(ICallbackService callbackService,
                                       IAzureMoveBlobHelper azureMoveBlobHelper,
                                       IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration,
                                       ILogger<HandleDeadLetterService> logger,
                                       IOptions<D365CallbackConfiguration> d365CallbackConfiguration)
        {
            _callbackService = callbackService;
            _azureMoveBlobHelper = azureMoveBlobHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
            _logger = logger;
            _d365CallbackConfiguration = d365CallbackConfiguration;
        }

        public async Task ProcessDeadLetter(string filePath, string subscriptionId, SubscriptionRequestMessage subscriptionRequestMessage)
        {
            ExternalNotificationEntity externalNotificationEntity = new()
            {
                ResponseStatusCode = _d365CallbackConfiguration.Value.FailedStatusCode,
                ResponseDetails = $"Failed to notify hence subscription is inactive @Time: {DateTime.UtcNow}.",
                ResponseStateCode = 1
            };
                                                                            
            string entityPath = $"ukho_externalnotifications({subscriptionId})";
            ////string fileName = Path.GetFileName(filePath);

            _logger.LogInformation(EventIds.DeadLetterCallbackToD365Started.ToEventId(),
            "DeadLetter Callback to D365 using Dataverse start with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{subscriptionId} and _D365-Correlation-ID:{correlationId} , _X-Correlation-ID:{CorrelationId} and ExternalNotificationEntity :{externalNotificationEntity}", externalNotificationEntity.ResponseStatusCode, externalNotificationEntity.ResponseDetails, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, JsonConvert.SerializeObject(externalNotificationEntity));

            await _callbackService.CallbackToD365UsingDataverse(entityPath, externalNotificationEntity, subscriptionRequestMessage);

            ////_logger.LogInformation(EventIds.DeadLetterMoveBlobStarted.ToEventId(),
            ////"DeadLetter file moves to the destination container start FileName:{fileName} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", fileName, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            ////await _azureMoveBlobHelper.DeadLetterMoveBlob(_ensStorageConfiguration.Value, filePath);

            ////_logger.LogInformation(EventIds.DeadLetterMoveBlobCompleted.ToEventId(),
            ////"DeadLetter file moves to the destination container completed FileName:{fileName} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", fileName, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);
        }

        public async Task<DateTime> GetBlockBlobLastModifiedDate(string filePath)
        {
            return await _azureMoveBlobHelper.GetBlockBlobLastModifiedDate(_ensStorageConfiguration.Value, filePath);
        }
    }
}
