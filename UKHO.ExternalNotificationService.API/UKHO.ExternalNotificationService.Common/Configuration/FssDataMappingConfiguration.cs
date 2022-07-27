using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class FssDataMappingConfiguration
    {
        public ICollection<SourceConfiguration> Sources { get; set; }
        public string EventHostName { get; set; }
        public string PublishHostName { get; set; }

        public class SourceConfiguration
        {
            public string BusinessUnit { get; set; }
            public string Source { get; set; }
        }
    }
}
