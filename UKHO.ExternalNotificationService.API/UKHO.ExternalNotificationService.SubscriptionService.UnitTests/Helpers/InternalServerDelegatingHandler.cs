﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Helpers
{
    public class InternalServerDelegatingHandler :DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.Headers.Add("retry-after", "3600");
            httpResponse.RequestMessage = new HttpRequestMessage();
            httpResponse.RequestMessage.Headers.Add("x-correlation-id", "");
            httpResponse.StatusCode = HttpStatusCode.InternalServerError;
            return Task.FromResult(httpResponse);
        }
    }
}
