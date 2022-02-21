using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public class SubscriptionServiceJob
    {
        private readonly ISubscriptionServiceData _subscriptionServiceData;
        private readonly ILogger<SubscriptionServiceJob> _logger;
        private readonly ILogger<CallbackService> _callbackLogger;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly IOptions<D365CallbackConfiguration> _D365CallbackConfiguration;
        private readonly CallbackService _callbackService;
        private readonly IHttpClientFactory _httpClientFactory;

        public SubscriptionServiceJob(ISubscriptionServiceData subscriptionServiceData, IAuthTokenProvider authTokenProvider,
            ILogger<SubscriptionServiceJob> logger, ILogger<CallbackService> callbackLogger ,
            IOptions<D365CallbackConfiguration> D365CallbackConfiguration, IHttpClientFactory httpClientFactory)
        {
            _subscriptionServiceData = subscriptionServiceData;
            _authTokenProvider = authTokenProvider;
            _callbackLogger = callbackLogger;
            _logger = logger;
            _D365CallbackConfiguration = D365CallbackConfiguration;
            _httpClientFactory = httpClientFactory;
            _callbackService = new CallbackService(_httpClientFactory, _authTokenProvider, _callbackLogger);
        }

        public async Task ProcessQueueMessage([QueueTrigger("%SubscriptionStorageConfiguration:QueueName%")] QueueMessage message)
        {            
            SubscriptionRequestMessage subscriptionMessage = message.Body.ToObjectFromJson<SubscriptionRequestMessage>();
            _logger.LogInformation(EventIds.CreateSubscriptionRequestStart.ToEventId(),
                    "Subscription provisioning request started for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

            SubscriptionRequestResult subscriptionRequestResult = new(subscriptionMessage);
            if (subscriptionMessage.IsActive)
            {
                ExternalNotificationEntity externalNotificationEntity = new()
                {
                    LastResponseStatusCode = _D365CallbackConfiguration.Value.SucceededStatusCode,
                    ResponseDetails = Convert.ToString(DateTime.UtcNow)
                };
                try
                {
                    await _subscriptionServiceData.CreateOrUpdateSubscription(subscriptionMessage, CancellationToken.None);
                    subscriptionRequestResult.ProvisioningState = "Succeeded";      
                }
                catch (Exception e)
                {
                    subscriptionRequestResult.ProvisioningState = e.Message;
                    _logger.LogError(EventIds.CreateSubscriptionRequestError.ToEventId(),
                   "Subscription provisioning request failed with Exception:{e} with SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", e.Message, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                }

                //Callback to D365
                _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
              "CallbackToD365UsingDataverse start with SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

                string entityPath = $"ukho_externalnotifications({subscriptionMessage.SubscriptionId})";
                await _callbackService.CallbackToD365UsingDataverse(entityPath, externalNotificationEntity, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            }
            
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                    "Subscription provisioning request Completed for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

        }
    }
}
