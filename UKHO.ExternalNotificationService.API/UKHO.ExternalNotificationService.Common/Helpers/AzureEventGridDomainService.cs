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
        
        public async Task<EventSubscription> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicStart.ToEventId(),
                    "Create or update azure event domain topic started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);
            
            EventGridManagementClient eventGridMgmtClient = await GetEventGridClient(_eventGridDomainConfig.SubscriptionId, cancellationToken);           
            DomainTopic topic = await GetDomainTopic(eventGridMgmtClient, subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);
            string eventSubscriptionScope = topic.Id;

            string deadLetterDestinationResourceId = $"/subscriptions/{_eventGridDomainConfig.SubscriptionId}/resourceGroups/{_eventGridDomainConfig.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{_eventGridDomainConfig.StorageAccountName}";

            EventSubscription eventSubscription = new EventSubscription() {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = subscriptionRequestMessage.WebhookUrl
                },
                // The below are all optional settings
                EventDeliverySchema = EventDeliverySchema.EventGridSchema,
                /* Retry policy decides when an event can be marked as expired. 
                   The default retry policy keeps the event alive for 24 hrs (=1440 mins or 30 retries with exponential backoffs)
                   An event is marked as expired once any of the retry policy limits are exceeded. 
                   Note: The below configuration for the retry policy will cause events to expire after one delivery attempt. 
                   This is only to make it easier to help test/verify dead letter destinations quickly.
                */
                RetryPolicy = new RetryPolicy()
                {
                    MaxDeliveryAttempts = _eventGridDomainConfig.MaxDeliveryAttempts,
                    EventTimeToLiveInMinutes = _eventGridDomainConfig.EventTimeToLiveInMinutes,
                },
                // With dead-letter destination configured, all expired events will be delivered to this destination.
                // Note: only Storage Blobs are supported as dead letter destinations as of now.
                DeadLetterDestination = new StorageBlobDeadLetterDestination()
                {
                    ResourceId = deadLetterDestinationResourceId,
                    BlobContainerName = _eventGridDomainConfig.StorageContainerName,
                }
            };
            EventSubscription createdOrUpdatedEventSubscription = await eventGridMgmtClient.EventSubscriptions.CreateOrUpdateAsync(eventSubscriptionScope, subscriptionRequestMessage.SubscriptionId, eventSubscription, CancellationToken.None);

            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicCompleted.ToEventId(),
                    "Create or update azure event domain topic completed for _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {topic}", subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, topic.Name);
            return createdOrUpdatedEventSubscription;
        }

        private static async Task<EventGridManagementClient> GetEventGridClient(string SubscriptionId, CancellationToken cancellationToken)
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { "https://management.azure.com/.default" });

            AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext, cancellationToken);
            TokenCredentials credential = new(tokenResult.Token);

            EventGridManagementClient _egClient = new(credential)
            {
                SubscriptionId = SubscriptionId
            };

            return _egClient;
        }

        protected virtual async Task<DomainTopic> GetDomainTopic(EventGridManagementClient eventGridMgmtClient, string NotificationTypeTopicName, CancellationToken cancellationToken)
        {
            return await eventGridMgmtClient.DomainTopics.CreateOrUpdateAsync(_eventGridDomainConfig.ResourceGroup, _eventGridDomainConfig.EventGridDomainName, NotificationTypeTopicName, cancellationToken);
        }
    }
}
