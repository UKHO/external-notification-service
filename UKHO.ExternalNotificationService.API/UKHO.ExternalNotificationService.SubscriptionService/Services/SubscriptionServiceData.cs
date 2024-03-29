﻿using Microsoft.Azure.Management.EventGrid.Models;
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

        public async Task<EventSubscription> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateSubscriptionServiceStart.ToEventId(),
                    "Create Subscription service started for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            EventSubscription response = await _azureEventGridDomainService.CreateOrUpdateSubscription(subscriptionMessage, cancellationToken);
            _logger.LogInformation(EventIds.CreateSubscriptionServiceCompleted.ToEventId(),
                    "Create Subscription service completed for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            return response;
        }

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
