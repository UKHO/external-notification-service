using Azure.ResourceManager.EventGrid;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public class SubscriptionServiceData: ISubscriptionServiceData
    {
        private readonly IAzureEventGridDomainService _azureEventGridDomainService;
        private readonly ILogger<SubscriptionServiceData> _logger;

        public SubscriptionServiceData(IAzureEventGridDomainService azureEventGridDomainService, ILogger<SubscriptionServiceData> logger)
        {
            _azureEventGridDomainService = azureEventGridDomainService;
            _logger = logger;
        }

        /// <summary>
        /// A Wrapper method to create or update a subscription, using the AzureEventGridDomainService.
        /// </summary>
        /// <param name="subscriptionRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The domain topic event subscription resource.</returns>
        public async Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(EventIds.CreateSubscriptionServiceStart.ToEventId(),
                    "Create Subscription service started for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            DomainTopicEventSubscriptionResource response = await _azureEventGridDomainService.CreateOrUpdateSubscription(subscriptionMessage, cancellationToken);
            _logger.LogInformation(EventIds.CreateSubscriptionServiceCompleted.ToEventId(),
                    "Create Subscription service completed for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            return response;
        }

        /// <summary>
        /// A Wrapper method to delete a subscription, using the AzureEventGridDomainService.
        /// Changes are due to the new Azure SDK for EventGrid.
        /// </summary>
        /// <param name="subscriptionRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.DeleteSubscriptionServiceEventStart.ToEventId(),
                    "Delete Subscription service started for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
           await _azureEventGridDomainService.DeleteSubscription(subscriptionMessage, cancellationToken);
            _logger.LogInformation(EventIds.DeleteSubscriptionServiceEventCompleted.ToEventId(),
                    "Delete Subscription service completed for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);          
        }
    }
}
