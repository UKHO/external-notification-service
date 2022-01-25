using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Response
{
    [ExcludeFromCodeCoverage]
    public class ErrorDescription
    {
        public string CorrelationId { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
    }
}
