using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Helper;
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
                ExtractD365Payload.D365PayloadDetails(payload, postEntityImageKey, out InputParameter inputParameter, out EntityImage postEntityImage);
                IEnumerable<D365Attribute> attributes = ExtractD365Payload.D365AttributeDetails(inputParameter, postEntityImage);

                D365Attribute validAttributeKey = attributes.FirstOrDefault(a => a.Key == attributeKey);
                return (validAttributeKey != null && !string.IsNullOrWhiteSpace(validAttributeKey.Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ContainsFormattedValue(this D365Payload payload, string postEntityImageKey, string formattedKey)
        {
            if (IgnoreRuleForValidation(payload))
            { return true; }
            try
            {
                ExtractD365Payload.D365PayloadDetails(payload, postEntityImageKey, out InputParameter inputParameter, out EntityImage postEntityImage);
                IEnumerable<FormattedValue> formattedValues = ExtractD365Payload.FormattedValueDetails(inputParameter, postEntityImage);

                FormattedValue validFormattedKey = formattedValues.FirstOrDefault(a => a.Key == formattedKey);
                return (validFormattedKey != null && !string.IsNullOrWhiteSpace(validFormattedKey.Value.ToString()));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IgnoreRuleForValidation(D365Payload payload)
        {
            return string.IsNullOrWhiteSpace(Convert.ToString(payload.InputParameters)) || string.IsNullOrWhiteSpace(Convert.ToString(payload.PostEntityImages));
        }
    }
}
