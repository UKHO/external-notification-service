using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Response
{
    [ExcludeFromCodeCoverage]
    public class InternalServerError
    {
        public string CorrelationId { get; set; }
        public string Detail { get; set; }
    }
}
