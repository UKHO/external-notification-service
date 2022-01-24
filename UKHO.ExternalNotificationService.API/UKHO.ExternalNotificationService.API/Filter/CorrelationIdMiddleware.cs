
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace UKHO.ExternalNotificationService.API.Filter
{
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdMiddleware
    {
        public const string XCorrelationIdHeaderKey = "X-Correlation-ID";
    }
}
