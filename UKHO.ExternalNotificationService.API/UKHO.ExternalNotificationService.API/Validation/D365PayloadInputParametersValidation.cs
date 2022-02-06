using System;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public static class D365PayloadInputParametersValidation
    {
        public static bool IsValidSubscriptionId(this D365Payload payload)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == "SubscriptionImage");
                var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.ImageValue?.Attributes ?? new D365Attribute[0]);

                var externalNotificationSubscriptionKey = attributes.FirstOrDefault(a => a.Key == "ukho_externalnotificationid");
                return (externalNotificationSubscriptionKey != null && !string.IsNullOrWhiteSpace(attributes.FirstOrDefault(a => a.Key == "ukho_externalnotificationid").Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidNotificationType(this D365Payload payload)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == "SubscriptionImage");
                var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.ImageValue?.FormattedValues ?? new FormattedValue[0]);

                object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.Key == "ukho_subscriptiontype");
                return (formattedSubscriptionType != null && !string.IsNullOrWhiteSpace(formattedValues.FirstOrDefault(a => a.Key == "ukho_subscriptiontype").Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidWebhookUrl(this D365Payload payload)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == "SubscriptionImage");
                var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.ImageValue?.Attributes ?? new D365Attribute[0]);

                var webhookurl = attributes.FirstOrDefault(a => a.Key == "ukho_webhookurl");
                return (webhookurl != null && !string.IsNullOrWhiteSpace(attributes.FirstOrDefault(a => a.Key == "ukho_webhookurl").Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidStatus(this D365Payload payload)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == "SubscriptionImage");
                var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.ImageValue?.FormattedValues ?? new FormattedValue[0]);

                string stateCode = formattedValues.FirstOrDefault(a => a.Key == "statecode")?.Value.ToString();
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
