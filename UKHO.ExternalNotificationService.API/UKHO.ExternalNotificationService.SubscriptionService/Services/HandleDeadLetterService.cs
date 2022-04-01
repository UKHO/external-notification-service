using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public class HandleDeadLetterService : IHandleDeadLetterService
    {
        private readonly ICallbackService _callbackService;
        private readonly IAzureMoveBlobHelper _azureMoveBlobHelper;
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;
        private readonly ILogger<HandleDeadLetterService> _logger;

        public HandleDeadLetterService(ICallbackService callbackService,
                                       IAzureMoveBlobHelper azureMoveBlobHelper,
                                       IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration,
                                       ILogger<HandleDeadLetterService> logger)
        {
            _callbackService = callbackService;
            _azureMoveBlobHelper = azureMoveBlobHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
            _logger = logger;
        }

        public async Task ProcessDeadLetter(string filePath, string subscriptionId, SubscriptionRequestMessage subscriptionRequestMessage)
        {
            ExternalNotificationEntity externalNotificationEntity = new() { ResponseStatusCode = 832930001, ResponseStateCode = 1};
            string entityPath = $"ukho_externalnotifications({subscriptionId})";
            string fileName = Path.GetFileName(filePath);

            _logger.LogInformation(EventIds.DeadLetterCallbackToD365Started.ToEventId(),
            "DeadLetter Callback to D365 using Dataverse start with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", externalNotificationEntity.ResponseStatusCode, externalNotificationEntity.ResponseDetails, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _callbackService.CallbackToD365UsingDataverse(entityPath, externalNotificationEntity, subscriptionRequestMessage);

            _logger.LogInformation(EventIds.DeadLetterMoveBlobStarted.ToEventId(),
            "DeadLetter file moves to the destination container start FileName:{fileName} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", fileName, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _azureMoveBlobHelper.DeadLetterMoveBlob(_ensStorageConfiguration.Value, filePath);

            _logger.LogInformation(EventIds.DeadLetterMoveBlobCompleted.ToEventId(),
            "DeadLetter file moves to the destination container completed FileName:{fileName} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", fileName, subscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);
        }
    }
}
