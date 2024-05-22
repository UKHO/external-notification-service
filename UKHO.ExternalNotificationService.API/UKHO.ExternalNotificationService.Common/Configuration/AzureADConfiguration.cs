using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AzureADConfiguration
    {
        public string MicrosoftOnlineLoginUrl { get; set; } = string.Empty;

        public string TenantId { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;
    }
}
