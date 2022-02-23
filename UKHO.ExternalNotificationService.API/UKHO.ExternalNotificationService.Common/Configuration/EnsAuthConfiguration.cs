using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AzureADConfiguration
    {
        public string MicrosoftOnlineLoginUrl { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}

