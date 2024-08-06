using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Models.Response
{
    [ExcludeFromCodeCoverage]
    public class Error
    {
        public string Source { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Source} - {Description}";
        }
    }
}
