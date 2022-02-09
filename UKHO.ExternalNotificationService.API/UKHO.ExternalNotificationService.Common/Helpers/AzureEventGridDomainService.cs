using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public class AzureEventGridDomainService : IAzureEventGridDomainService
    {
        private readonly EventGridDomainConfiguration _eventGridDomainConfig;
        private readonly ILogger<AzureEventGridDomainService> _logger;

        public AzureEventGridDomainService(IOptions<EventGridDomainConfiguration> eventGridDomainConfig, ILogger<AzureEventGridDomainService> logger)
        {
            _eventGridDomainConfig = eventGridDomainConfig.Value;
            _logger = logger;
        }

        public async Task<string> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicStart.ToEventId(),
                    "Create azure event domain topic started for _X-Correlation-ID:{CorrelationId} with Event domain topic {topic}", subscriptionMessage.CorrelationId, subscriptionMessage.NotificationTypeTopicName);
            EventGridManagementClient eventGridMgmtClient = await GetEventGridClient(_eventGridDomainConfig.SubscriptionId, cancellationToken);
            DomainTopic topic = await eventGridMgmtClient.DomainTopics.CreateOrUpdateAsync(_eventGridDomainConfig.ResourceGroup, _eventGridDomainConfig.EventGridDomainName, subscriptionMessage.NotificationTypeTopicName, cancellationToken);
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicCompleted.ToEventId(),
                    "Create azure event domain topic completed for _X-Correlation-ID:{CorrelationId} with Event domain topic {topic}", subscriptionMessage.CorrelationId, topic.Name);
            return topic.Id;
        }

        private async Task<EventGridManagementClient> GetEventGridClient(string SubscriptionId, CancellationToken cancellationToken)
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { "https://management.azure.com/.default" });

            var tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext, cancellationToken);
            var credential = new TokenCredentials(tokenResult.Token);

            EventGridManagementClient _egClient = new(credential)
            {
                SubscriptionId = SubscriptionId
            };

            return _egClient;
        }
    }
}
