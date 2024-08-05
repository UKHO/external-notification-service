using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventHubLoggingConfiguration
    {
        public string MinimumLoggingLevel { get; set; } = string.Empty;
        public string UkhoMinimumLoggingLevel { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string EntityPath { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
