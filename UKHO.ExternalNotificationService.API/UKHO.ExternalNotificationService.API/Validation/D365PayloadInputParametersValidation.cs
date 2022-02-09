using System;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public static class D365PayloadInputParametersValidation
    {
        public static bool IsValidAttribute(this D365Payload payload,string postEntityImageKey, string attributeKey)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == postEntityImageKey);
                var attributes = inputParameter.Value.Attributes.Concat(postEntityImage?.Value?.Attributes ?? Array.Empty<D365Attribute>());

                var validAttributeKey = attributes.FirstOrDefault(a => a.Key == attributeKey);
                return (validAttributeKey != null && !string.IsNullOrWhiteSpace(attributes.FirstOrDefault(a => a.Key == attributeKey).Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsValidFormatted(this D365Payload payload, string postEntityImageKey, string formattedKey)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                var inputParameter = payload.InputParameters.Single();
                var postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == postEntityImageKey);
                var formattedValues = inputParameter.Value.FormattedValues.Concat(postEntityImage?.Value?.FormattedValues ?? Array.Empty<FormattedValue>());

                object validFormattedKey = formattedValues.FirstOrDefault(a => a.Key == formattedKey);
                return (validFormattedKey != null && !string.IsNullOrWhiteSpace(formattedValues.FirstOrDefault(a => a.Key == formattedKey).Value.ToString()));
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
