using System;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Extensions
{
    public static class D365InputParametersExtensions
    {
        public static bool IsValidSubscriptionId(this D365Payload payload)
        {
            try
            {
                var inputParameter = GetInputParameter(payload);
                var postEntityImage = GetPostEntityImage(payload);
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
            try
            {
                var inputParameter = GetInputParameter(payload);
                var postEntityImage = GetPostEntityImage(payload);
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
            try
            {
                var inputParameter = GetInputParameter(payload);
                var postEntityImage = GetPostEntityImage(payload);
                var attributes = inputParameter.value.Attributes.Concat(postEntityImage?.value?.Attributes ?? new D365Attribute[0]);

                var webhookurl = attributes.FirstOrDefault(a => a.key == "ukho_webhookurl").value;
                return !string.IsNullOrWhiteSpace(Convert.ToString(webhookurl));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static InputParameter GetInputParameter(D365Payload payload)
        {
            InputParameter inputParameter;
            if (payload.InputParameters != null)
            {
                inputParameter = payload.InputParameters.Single();
            }
            else
            {
                inputParameter = new InputParameter() { value = new InputParameterValue() { Attributes = new D365Attribute[] { }, FormattedValues = new FormattedValue[]{}}};
            }
            return inputParameter;
        }
        private static EntityImage GetPostEntityImage(D365Payload payload)
        {
            EntityImage entityImage;
            if (payload.PostEntityImages != null)
            {
                entityImage = payload.PostEntityImages.SingleOrDefault(i => i.key == "SubscriptionImage");
            }
            else
            {
                entityImage = new EntityImage() { value = new EntityImageValue() { Attributes = new D365Attribute[] { }, FormattedValues = new FormattedValue[]{}}};
            }
            return entityImage;
        }
    }
}
