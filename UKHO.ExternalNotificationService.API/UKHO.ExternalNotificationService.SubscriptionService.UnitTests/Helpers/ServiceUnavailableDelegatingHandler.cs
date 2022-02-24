using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Helpers
{
    public class ServiceUnavailableDelegatingHandler : DelegatingHandler
    {        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage
            {
                RequestMessage = new HttpRequestMessage(),
                StatusCode = HttpStatusCode.ServiceUnavailable
            };
            return Task.FromResult(httpResponse);
        }
    
}
}
