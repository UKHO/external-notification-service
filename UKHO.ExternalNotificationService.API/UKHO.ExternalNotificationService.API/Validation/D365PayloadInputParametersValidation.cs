using System;
using System.Linq;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public static class D365PayloadInputParametersValidation
    {
        public static bool IsValidSubscriptionId(this D365Payload payload, IOptions<D365PayloadKeyConfiguration> d365PayloadKeyConfiguration)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == d365PayloadKeyConfiguration.Value.PostEntityImageKey);
                var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.ImageValue?.Attributes ?? new D365Attribute[0]);

                var externalNotificationSubscriptionKey = attributes.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.SubscriptionIdKey);
                return (externalNotificationSubscriptionKey != null && !string.IsNullOrWhiteSpace(attributes.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.SubscriptionIdKey).Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidNotificationType(this D365Payload payload, IOptions<D365PayloadKeyConfiguration> d365PayloadKeyConfiguration)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == d365PayloadKeyConfiguration.Value.PostEntityImageKey);
                var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.ImageValue?.FormattedValues ?? new FormattedValue[0]);

                object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.NotificationTypeKey);
                return (formattedSubscriptionType != null && !string.IsNullOrWhiteSpace(formattedValues.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.NotificationTypeKey).Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidWebhookUrl(this D365Payload payload, IOptions<D365PayloadKeyConfiguration> d365PayloadKeyConfiguration)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == d365PayloadKeyConfiguration.Value.PostEntityImageKey);
                var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.ImageValue?.Attributes ?? new D365Attribute[0]);

                var webhookurl = attributes.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.WebhookUrlKey);
                return (webhookurl != null && !string.IsNullOrWhiteSpace(attributes.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.WebhookUrlKey).Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidStatus(this D365Payload payload, IOptions<D365PayloadKeyConfiguration> d365PayloadKeyConfiguration)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == d365PayloadKeyConfiguration.Value.PostEntityImageKey);
                var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.ImageValue?.FormattedValues ?? new FormattedValue[0]);

                string stateCode = formattedValues.FirstOrDefault(a => a.Key == d365PayloadKeyConfiguration.Value.IsActiveKey)?.Value.ToString();
                return !string.IsNullOrWhiteSpace(stateCode);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static bool IgnoreRuleForValidation(D365Payload payload)
        {
            return (string.IsNullOrWhiteSpace(Convert.ToString(payload.InputParameters)) || string.IsNullOrWhiteSpace(Convert.ToString(payload.PostEntityImages)));
        }
    }
}
