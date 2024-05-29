using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureEventGridDomainService : IAzureEventGridDomainService
    {
        private readonly EventGridDomainConfiguration _eventGridDomainConfig;
        private readonly IEventSubscriptionConfiguration _eventSubscriptionConfiguration;
        private readonly ILogger<AzureEventGridDomainService> _logger;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public AzureEventGridDomainService(IOptions<EventGridDomainConfiguration> eventGridDomainConfig, IEventSubscriptionConfiguration eventSubscriptionConfiguration, ILogger<AzureEventGridDomainService> logger)
        {
            _eventGridDomainConfig = eventGridDomainConfig.Value;
            _eventSubscriptionConfiguration = eventSubscriptionConfiguration;
            _logger = logger;            
        }
        
        public async Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicStart.ToEventId(),
                    "Create or update azure event domain topic started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);

            DomainTopicResource topic = await GetDomainTopic(subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);

            DomainTopicEventSubscriptionResource eventSubscriptionResult = await EditDomainTopicEventSubscription(
                            topic,
                            subscriptionRequestMessage,
                            cancellationToken);

            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicCompleted.ToEventId(),
                    "Create or update azure event domain topic and subscription completed for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} for Event domain topic {topic}",
                    subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, topic.Id.Name);

            return eventSubscriptionResult;
        }

        public async Task DeleteSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.DeleteAzureEventDomainSubscriptionStart.ToEventId(),
                    "Delete azure event grid domain subscription started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);

            DomainTopicResource topic = await GetDomainTopic(subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);

            DomainTopicEventSubscriptionResource topicEventSubscription = await topic.GetDomainTopicEventSubscriptionAsync(subscriptionRequestMessage.SubscriptionId, cancellationToken);
            await topicEventSubscription.DeleteAsync(WaitUntil.Completed, cancellationToken);

            _logger.LogInformation(EventIds.DeleteAzureEventDomainSubscriptionCompleted.ToEventId(),
                    "Delete azure event grid domain subscription completed for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} for Event domain topic {topic}",
                    subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, topic.Id.Name);           
        }

        public async Task PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            EventGridPublisherClient client = new(new Uri(_eventGridDomainConfig.EventGridDomainEndpoint),
                                                  new AzureKeyCredential(_eventGridDomainConfig.EventGridDomainAccessKey));
            List<CloudEvent> listCloudEvent = new() { cloudEvent };


            try
            {
                _logger.LogInformation(EventIds.ENSEventPublishStart.ToEventId(), "External notification service event publish started for event:{cloudEvent}, subject:{subject} and _X-Correlation-ID:{correlationId}.", JsonSerializer.Serialize(cloudEvent), cloudEvent.Subject, correlationId);
                await client.SendEventsAsync(listCloudEvent, cancellationToken);
                _logger.LogInformation(EventIds.ENSEventPublishCompleted.ToEventId(), "External notification service event publish successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", cloudEvent.Subject, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ENSEventNotPublished.ToEventId(), "External notification service event publish failed for subject:{subject} and _X-Correlation-ID:{correlationId} with error:{Message}.", cloudEvent.Subject, correlationId, ex.Message);
            }
        }

        // We have to do this because the data object is likely to be a JsonElement.
        public T? ConvertObjectTo<T>(object data) where T : class
        {
            T obj = default!;
            string jsonString = JsonSerializer.Serialize(data);
            if (jsonString != null)
            {
                obj = JsonSerializer.Deserialize<T>(jsonString,_options)!;
            }
            return obj;
        }

        private ArmClient GetArmClient()
        {
            DefaultAzureCredential azureCredential = new();

            var result = new ArmClient(azureCredential, _eventGridDomainConfig.SubscriptionId);

            return result;
        }

        protected virtual async Task<DomainTopicResource> GetDomainTopic(string notificationTypeTopicName, CancellationToken cancellationToken=default)
        {
            ArmClient client = GetArmClient();
            ResourceIdentifier ri = EventGridDomainResource.CreateResourceIdentifier(
                    _eventGridDomainConfig.SubscriptionId,
                    _eventGridDomainConfig.ResourceGroup,
                    _eventGridDomainConfig.EventGridDomainName);

            EventGridDomainResource eventGridDomain = client.GetEventGridDomainResource(ri);

            DomainTopicCollection collection = eventGridDomain.GetDomainTopics();
            ArmOperation<DomainTopicResource> topic = await collection.CreateOrUpdateAsync(WaitUntil.Completed, notificationTypeTopicName, cancellationToken);
            return topic.Value;
        }

        protected async Task<DomainTopicEventSubscriptionResource> EditDomainTopicEventSubscription(DomainTopicResource topic, SubscriptionRequestMessage message, CancellationToken cancellationToken = default)
        {
            EventGridSubscriptionData eventSubscriptionData = _eventSubscriptionConfiguration.SetEventSubscription(message);

            ArmOperation<DomainTopicEventSubscriptionResource> eventSubscriptionResult = await topic.GetDomainTopicEventSubscriptions().CreateOrUpdateAsync(
                          WaitUntil.Completed,
                          message.SubscriptionId,
                          eventSubscriptionData, cancellationToken);
            return eventSubscriptionResult.Value;
        }
    }
}
