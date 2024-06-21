using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Models.Response
{
    [ExcludeFromCodeCoverage]
    public class InternalServerError
    {
        public string CorrelationId { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
