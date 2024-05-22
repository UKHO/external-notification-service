using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public static class ExtractD365Payload
    {
        public static void D365PayloadDetails(D365Payload payload, string postEntityImageKey, out InputParameter inputParameter, out EntityImage? postEntityImage)
        {
            inputParameter = payload.InputParameters.Single();
            postEntityImage = payload.PostEntityImages.SingleOrDefault(i => i.Key == postEntityImageKey);
        }

        public static IEnumerable<D365Attribute> D365AttributeDetails(InputParameter inputParameter, EntityImage postEntityImage)
        {
            return inputParameter.Value?.Attributes.Concat(postEntityImage?.Value?.Attributes ?? Array.Empty<D365Attribute>())!;
        }

        public static IEnumerable<FormattedValue> FormattedValueDetails(InputParameter inputParameter, EntityImage postEntityImage)
        {
            return inputParameter.Value?.FormattedValues.Concat(postEntityImage?.Value?.FormattedValues ?? Array.Empty<FormattedValue>())!;
        } 
    }
}
