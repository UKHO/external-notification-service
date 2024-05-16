using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Models.Response
{
    [ExcludeFromCodeCoverage]
    public class ErrorDescription
    {
        public string CorrelationId { get; set; } = string.Empty;
        public List<Error> Errors { get; set; } = new List<Error>();
    }
}
