using Azure.Storage.Queues.Models;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public class SubscriptionServiceJob
    {
        private readonly ISubscriptionServiceData _subscriptionServiceData;
        private readonly ILogger<SubscriptionServiceJob> _logger;

        public SubscriptionServiceJob(ISubscriptionServiceData subscriptionServiceData, ILogger<SubscriptionServiceJob> logger)
        {
            _subscriptionServiceData = subscriptionServiceData;
            _logger = logger;
        }

        public async Task ProcessQueueMessage([QueueTrigger("%SubscriptionStorageConfiguration:QueueName%")] QueueMessage message)
        {            
            SubscriptionRequestMessage subscriptionMessage = message.Body.ToObjectFromJson<SubscriptionRequestMessage>();
            EventSubscription eventSubscription;
            _logger.LogInformation(EventIds.CreateSubscriptionRequestStart.ToEventId(),
                    "Subscription provisioning request started for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

            SubscriptionRequestResult subscriptionRequestResult = new(subscriptionMessage);
            if (subscriptionMessage.IsActive)
            {
                try
                {
                    eventSubscription = await _subscriptionServiceData.CreateOrUpdateSubscription(subscriptionMessage, CancellationToken.None);
                    subscriptionRequestResult.ProvisioningState = "Succeeded";
                }
                catch (Exception e)
                {
                    subscriptionRequestResult.ProvisioningState = e.Message;
                    _logger.LogError(EventIds.CreateSubscriptionRequestError.ToEventId(),
                   "Subscription provisioning request failed with Exception:{e} with SubscriptionId:{SubscriptionId} _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", e.Message, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                }
            }
            
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                    "Subscription provisioning request Completed for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

        }
    }
}
