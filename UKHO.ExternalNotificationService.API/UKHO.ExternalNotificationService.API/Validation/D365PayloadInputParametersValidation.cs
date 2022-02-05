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
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.key == "SubscriptionImage");
                var attributes = inputParameter.value.Attributes.Concat(postEntityImage?.value?.Attributes ?? new D365Attribute[0]);

                var externalNotificationSubscriptionKey = attributes.FirstOrDefault(a => a.key == "ukho_externalnotificationid");
                return (externalNotificationSubscriptionKey != null && !string.IsNullOrWhiteSpace(Convert.ToString(attributes.FirstOrDefault(a => a.key == "ukho_externalnotificationid").value)));
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
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.key == "SubscriptionImage");
                var formattedValues = inputParameter.value.FormattedValues.Concat(postEntityImage?.value?.FormattedValues ?? new FormattedValue[0]);

                object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.key == "ukho_subscriptiontype");
                return (formattedSubscriptionType != null && !string.IsNullOrWhiteSpace(Convert.ToString(formattedValues.FirstOrDefault(a => a.key == "ukho_subscriptiontype").value)));
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
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.key == "SubscriptionImage");
                var attributes = inputParameter.value.Attributes.Concat(postEntityImage?.value?.Attributes ?? new D365Attribute[0]);

                var webhookurl = attributes.FirstOrDefault(a => a.key == "ukho_webhookurl");
                return (webhookurl != null && !string.IsNullOrWhiteSpace(Convert.ToString(attributes.FirstOrDefault(a => a.key == "ukho_webhookurl").value)));
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
