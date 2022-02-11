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
                    "Create Subscription service started for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            string response = await _azureEventGridDomainService.CreateOrUpdateSubscription(subscriptionMessage, cancellationToken);
            _logger.LogInformation(EventIds.CreateSubscriptionServiceCompleted.ToEventId(),
                    "Create Subscription service completed for _D365-Correlation-ID:{CorrelationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            return response;
        }
    }
}
