using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public async Task<string> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateSubscriptionServiceStart.ToEventId(),
                    "Create Subscription service started for _X-Correlation-ID:{CorrelationId}", subscriptionMessage.CorrelationId);
            var response = await _azureEventGridDomainService.CreateOrUpdateSubscription(subscriptionMessage, cancellationToken);
            _logger.LogInformation(EventIds.CreateSubscriptionServiceCompleted.ToEventId(),
                    "Create Subscription service completed for _X-Correlation-ID:{CorrelationId}", subscriptionMessage.CorrelationId);
            return response;
        }
    }
}
