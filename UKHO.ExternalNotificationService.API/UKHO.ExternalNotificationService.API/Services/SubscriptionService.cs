using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ID365PayloadValidator _d365PayloadValidator;
        private readonly IOptions<D365PayloadKeyConfiguration> _d365PayloadKeyConfiguration;

        public SubscriptionService(ID365PayloadValidator d365PayloadValidator,
                                   IOptions<D365PayloadKeyConfiguration> d365PayloadKeyConfiguration)
        {
            _d365PayloadValidator = d365PayloadValidator;
            _d365PayloadKeyConfiguration = d365PayloadKeyConfiguration;
        }

        public Task<ValidationResult> ValidateD365PayloadRequest(D365Payload d365Payload)
        {
            return _d365PayloadValidator.Validate(d365Payload);
        }

        public SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload d365Payload)
        {
            var inputParameter = d365Payload.InputParameters.Single();
            var postEntityImage = d365Payload.PostEntityImages.SingleOrDefault(i => i.Key == _d365PayloadKeyConfiguration.Value.PostEntityImageKey);
            var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.Value?.Attributes ?? new D365Attribute[0]);
            var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.Value?.FormattedValues ?? new FormattedValue[0]);

            string correlationId = d365Payload.CorrelationId;
            string stateCode = formattedValues.FirstOrDefault(a => a.Key == _d365PayloadKeyConfiguration.Value.IsActiveKey)?.Value.ToString();
            object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.Key == _d365PayloadKeyConfiguration.Value.NotificationTypeKey).Value;
            var externalNotificationSubscriptionId = attributes.FirstOrDefault(a => a.Key == _d365PayloadKeyConfiguration.Value.SubscriptionIdKey).Value;
            var webhookurl = attributes.FirstOrDefault(a => a.Key == _d365PayloadKeyConfiguration.Value.WebhookUrlKey).Value;

            return new SubscriptionRequest()
            {
                SubscriptionId = Convert.ToString(externalNotificationSubscriptionId),
                IsActive = string.Equals(stateCode, "Active", StringComparison.InvariantCultureIgnoreCase),
                WebhookUrl = Convert.ToString(webhookurl),
                NotificationType = Convert.ToString(formattedSubscriptionType),
                D365CorrelationId = correlationId
            };
        }
    }
}
