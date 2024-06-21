using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class FssDataMappingConfiguration
    {
        public ICollection<SourceConfiguration> Sources { get; set; } = new List<SourceConfiguration>();
        public string EventHostName { get; set; } = string.Empty;
        public string PublishHostName { get; set; } = string.Empty;

        public class SourceConfiguration
        {
            public string BusinessUnit { get; set; } = string.Empty;
            public string Source { get; set; } = string.Empty;
        }
    }
}
