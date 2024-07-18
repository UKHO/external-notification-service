using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventProcessorConfiguration
    {
        public int ScsEventPublishDelayInSeconds { get; set; }
    }
}
