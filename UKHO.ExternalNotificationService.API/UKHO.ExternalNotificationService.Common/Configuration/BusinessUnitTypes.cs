using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public static class BusinessUnitTypes
    {
        public static readonly IEnumerable<string> BusinessUnit = new List<string>
        {
            "AVCSData",
            "MaritimeSafetyInformation",
            "ADP",
            "AENP",
            "ARCS",
            "SENC"
        };
    }
}
