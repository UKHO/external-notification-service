using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ID365PayloadValidator _d365PayloadValidator;

        public SubscriptionService(ID365PayloadValidator d365PayloadValidator)
        {
            _d365PayloadValidator = d365PayloadValidator;
        }

        public Task<ValidationResult> ValidateD365PayloadRequest(D365Payload d365Payload)
        {
            return _d365PayloadValidator.Validate(d365Payload);
        }

        public SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload d365Payload)
        {
            var inputParameter = d365Payload.InputParameters.Single();
            var postEntityImage = d365Payload.PostEntityImages.SingleOrDefault(i => i.key == "SubscriptionImage");
            var attributes = inputParameter.value.Attributes.Concat(postEntityImage?.value?.Attributes ?? new D365Attribute[0]);
            var formattedValues = inputParameter.value.FormattedValues.Concat(postEntityImage?.value?.FormattedValues ?? new FormattedValue[0]);

            string correlationId = d365Payload.CorrelationId;
            string stateCode = formattedValues.FirstOrDefault(a => a.key == "statecode")?.value.ToString();
            object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.key == "ukho_subscriptiontype").value;
            var externalNotificationSubscriptionId = attributes.FirstOrDefault(a => a.key == "ukho_externalnotificationid").value;
            var webhookurl = attributes.FirstOrDefault(a => a.key == "ukho_webhookurl").value;

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
