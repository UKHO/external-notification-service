using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureEventGridDomainService : IAzureEventGridDomainService
    {
        private readonly EventGridDomainConfiguration _eventGridDomainConfig;
        private readonly ILogger<AzureEventGridDomainService> _logger;        

        public AzureEventGridDomainService(IOptions<EventGridDomainConfiguration> eventGridDomainConfig, ILogger<AzureEventGridDomainService> logger)
        {
            _eventGridDomainConfig = eventGridDomainConfig.Value;
            _logger = logger;            
        }
        
        public async Task<string> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicStart.ToEventId(),
                    "Create or update azure event domain topic started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);
            
            EventGridManagementClient eventGridMgmtClient = await GetEventGridClient(_eventGridDomainConfig.SubscriptionId, cancellationToken);           
            DomainTopic topic = await GetDomainTopic(eventGridMgmtClient, subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);

            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicCompleted.ToEventId(),
                    "Create or update azure event domain topic completed for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {topic}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, topic.Name);
            return topic.Id;
        }

        private static async Task<EventGridManagementClient> GetEventGridClient(string subscriptionId, CancellationToken cancellationToken)
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { "https://management.azure.com/.default" });

            AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext, cancellationToken);
            TokenCredentials credential = new(tokenResult.Token);

            EventGridManagementClient _egClient = new(credential)
            {
                SubscriptionId = subscriptionId
            };

            return _egClient;
        }

        protected virtual async Task<DomainTopic> GetDomainTopic(EventGridManagementClient eventGridMgmtClient, string notificationTypeTopicName, CancellationToken cancellationToken)
        {
            return await eventGridMgmtClient.DomainTopics.CreateOrUpdateAsync(_eventGridDomainConfig.ResourceGroup, _eventGridDomainConfig.EventGridDomainName, notificationTypeTopicName, cancellationToken);
        }
    }
}
