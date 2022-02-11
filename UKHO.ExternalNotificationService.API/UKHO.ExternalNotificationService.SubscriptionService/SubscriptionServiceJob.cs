using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "check web job is triggered or not using Azure Queue {message}", message.Body.ToString());
            SubscriptionRequestMessage subscriptionMessage = message.Body.ToObjectFromJson<SubscriptionRequestMessage>();
            _logger.LogInformation(EventIds.CreateSubscriptionRequestStart.ToEventId(),
                    "Create Subscription web job request started for _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId);

            SubscriptionRequestResult subscriptionRequestResult = new(subscriptionMessage);
            if (subscriptionMessage.IsActive)
            {
                await _subscriptionServiceData.CreateOrUpdateSubscription(subscriptionMessage, CancellationToken.None);
                subscriptionRequestResult.ProvisioningState = "Succeeded";
            }
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                    "Create Subscription web job request Completed for _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId);
        }
    }
}
