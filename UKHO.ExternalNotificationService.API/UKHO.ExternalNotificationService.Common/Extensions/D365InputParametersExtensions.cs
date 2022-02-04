using System;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Extensions
{
    public static class D365InputParametersExtensions
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

                var externalNotificationSubscriptionId = attributes.FirstOrDefault(a => a.key == "ukho_externalnotificationid").value;
                return !string.IsNullOrWhiteSpace(Convert.ToString(externalNotificationSubscriptionId));
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

                object formattedSubscriptionType = formattedValues.FirstOrDefault(a => a.key == "ukho_subscriptiontype").value;
                return !string.IsNullOrWhiteSpace(Convert.ToString(formattedSubscriptionType));
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

                var webhookurl = attributes.FirstOrDefault(a => a.key == "ukho_webhookurl").value;
                return !string.IsNullOrWhiteSpace(Convert.ToString(webhookurl));
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
