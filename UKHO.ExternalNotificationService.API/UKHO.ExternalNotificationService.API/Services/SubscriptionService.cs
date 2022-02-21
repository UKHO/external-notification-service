using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;


namespace UKHO.ExternalNotificationService.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ID365PayloadValidator _d365PayloadValidator;
        private readonly IAzureMessageQueueHelper _azureMessageQueueHelper;
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;        
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(ID365PayloadValidator d365PayloadValidator, IAzureMessageQueueHelper azureMessageQueueHelper, IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration, ILogger<SubscriptionService> logger)
        {
            _d365PayloadValidator = d365PayloadValidator;
            _azureMessageQueueHelper = azureMessageQueueHelper;
            _ensStorageConfiguration = ensStorageConfiguration;            
            _logger = logger;
        }

        public Task<ValidationResult> ValidateD365PayloadRequest(D365Payload d365Payload)
        {
            return _d365PayloadValidator.Validate(d365Payload);
        }

        public SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload d365Payload)
        {
            ExtractD365Payload.D365PayloadDetails(d365Payload, D365PayloadKeyConstant.PostEntityImageKey, out InputParameter inputParameter, out EntityImage postEntityImage);
            IEnumerable<D365Attribute> attributes = ExtractD365Payload.D365AttributeDetails(inputParameter, postEntityImage);
            IEnumerable<FormattedValue> formattedValues = ExtractD365Payload.FormattedValueDetails(inputParameter, postEntityImage);

            string correlationId = d365Payload.CorrelationId;
            string stateCode = formattedValues.FirstOrDefault(a => a.Key == D365PayloadKeyConstant.IsActiveKey)?.Value.ToString();
            object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.Key == D365PayloadKeyConstant.NotificationTypeKey).Value;
            object externalNotificationSubscriptionId = attributes.FirstOrDefault(a => a.Key == D365PayloadKeyConstant.SubscriptionIdKey).Value;
            object webhookurl = attributes.FirstOrDefault(a => a.Key == D365PayloadKeyConstant.WebhookUrlKey).Value;

            return new SubscriptionRequest()
            {
                SubscriptionId = Convert.ToString(externalNotificationSubscriptionId),
                IsActive = string.Equals(stateCode, "Active", StringComparison.InvariantCultureIgnoreCase),
                WebhookUrl = Convert.ToString(webhookurl),
                NotificationType = Convert.ToString(formattedSubscriptionType),
                D365CorrelationId = correlationId
            };            
        }

        public async Task AddSubscriptionRequest(SubscriptionRequest subscriptionRequest,NotificationType notificationType, string correlationId)
        {
            SubscriptionRequestMessage subscriptionRequestMessage = new()
            {
                SubscriptionId = subscriptionRequest.SubscriptionId,
                NotificationType = notificationType.Name,
                NotificationTypeTopicName = notificationType.TopicName,
                IsActive = subscriptionRequest.IsActive,
                WebhookUrl = subscriptionRequest.WebhookUrl,
                D365CorrelationId = subscriptionRequest.D365CorrelationId,
                CorrelationId = correlationId
            };         

            await _azureMessageQueueHelper.AddQueueMessage(_ensStorageConfiguration.Value, subscriptionRequestMessage);
            _logger.LogInformation(EventIds.AddedMessageInQueue.ToEventId(), "Subscription request message added in Queue for SubscriptionId:{subscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscriptionRequest.SubscriptionId, subscriptionRequest.D365CorrelationId, correlationId);
        }        
    }
}
