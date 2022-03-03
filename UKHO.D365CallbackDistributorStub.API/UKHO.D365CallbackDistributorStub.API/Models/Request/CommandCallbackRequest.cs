using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CommandCallbackRequest
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string? SubscriptionId { get; set; }
    }
}
