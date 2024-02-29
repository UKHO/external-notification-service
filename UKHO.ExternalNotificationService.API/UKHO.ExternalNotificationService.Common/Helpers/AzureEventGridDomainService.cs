using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public AzureEventGridDomainService(IOptions<EventGridDomainConfiguration> eventGridDomainConfig, IEventSubscriptionConfiguration eventSubscriptionConfiguration, ILogger<AzureEventGridDomainService> logger)
        {
            _eventGridDomainConfig = eventGridDomainConfig.Value;
            _eventSubscriptionConfiguration = eventSubscriptionConfiguration;
            _logger = logger;            
        }
        
        public async Task<DomainTopicEventSubscriptionResource> CreateOrUpdateSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicStart.ToEventId(),
                    "Create or update azure event domain topic started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);

            ArmClient client = await GetEventGridClient(_eventGridDomainConfig.SubscriptionId, cancellationToken);
           
            DomainTopicResource topic = await GetDomainTopic(client, subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);
            //string eventSubscriptionScope = topic.Id;

            EventGridSubscriptionData eventSubscriptionData = _eventSubscriptionConfiguration.SetEventSubscription(subscriptionRequestMessage);

            ArmOperation<DomainTopicEventSubscriptionResource> eventSubscriptionResult = await topic.GetDomainTopicEventSubscriptions().CreateOrUpdateAsync(
                          WaitUntil.Completed,
                          subscriptionRequestMessage.SubscriptionId,
                          eventSubscriptionData, cancellationToken);

            _logger.LogInformation(EventIds.CreateOrUpdateAzureEventDomainTopicCompleted.ToEventId(),
                    "Create or update azure event domain topic and subscription completed for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} for Event domain topic {topic}",
                    subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, topic.Id.Name);
            return eventSubscriptionResult.Value;
        }

        public async Task DeleteSubscription(SubscriptionRequestMessage subscriptionRequestMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation(EventIds.DeleteAzureEventDomainSubscriptionStart.ToEventId(),
                    "Delete azure event grid domain subscription started for SubscriptionId:{SubscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId} with Event domain topic {NotificationTypeTopicName}", subscriptionRequestMessage.SubscriptionId, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId, subscriptionRequestMessage.NotificationTypeTopicName);
            ArmClient client = await GetEventGridClient(_eventGridDomainConfig.SubscriptionId, cancellationToken);

            DomainTopicResource topic = await GetDomainTopic(client, subscriptionRequestMessage.NotificationTypeTopicName, cancellationToken);
            //string eventSubscriptionScope = topic.Id;

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

        public T JsonDeserialize<T>(object data)
        {
            string jsonString = JsonConvert.SerializeObject(data);
            T obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }

        private static async Task<ArmClient> GetEventGridClient(string subscriptionId, CancellationToken cancellationToken)
        {
            DefaultAzureCredential azureCredential = new();
            //TokenRequestContext tokenRequestContext = new(new string[] { "https://management.azure.com/.default" });

            //AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext,  cancellationToken);
            //TokenCredential credential = new(tokenResult.Token);
            //
            //return new(credential)
            //{
            //    SubscriptionId = subscriptionId
            //};

            var result = new ArmClient(azureCredential, subscriptionId);

            return await Task.FromResult(result);
        }

        protected virtual async Task<DomainTopicResource> GetDomainTopic(ArmClient client, string notificationTypeTopicName, CancellationToken cancellationToken)
        {
            ResourceIdentifier ri = EventGridDomainResource.CreateResourceIdentifier(
                    _eventGridDomainConfig.SubscriptionId,
                    _eventGridDomainConfig.ResourceGroup,
                    _eventGridDomainConfig.EventGridDomainName);

            EventGridDomainResource eventGridDomain = client.GetEventGridDomainResource(ri);

            DomainTopicCollection collection = eventGridDomain.GetDomainTopics();
            ArmOperation<DomainTopicResource> topic = await collection.CreateOrUpdateAsync(WaitUntil.Completed, notificationTypeTopicName, cancellationToken);
            return topic.Value;
        }
    }
}
