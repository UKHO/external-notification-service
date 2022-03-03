using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class FailCallbackRequest
    {
        public HttpStatusCode httpStatusCode { get; set; }
        public string? SubscriptionId { get; set; }
    }
}
