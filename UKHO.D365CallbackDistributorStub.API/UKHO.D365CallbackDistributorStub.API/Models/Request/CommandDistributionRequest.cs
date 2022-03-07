using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CommandDistributionRequest
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string? Subject { get; set; }
    }
}
