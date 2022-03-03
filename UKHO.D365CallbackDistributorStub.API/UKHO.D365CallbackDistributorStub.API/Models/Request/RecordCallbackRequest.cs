using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class RecordCallbackRequest
    {
        public CallbackRequest? CallBackRequest { get; set; }
        public string? SubscriptionId { get; set; }
        public Guid Guid { get; set; }
        public string? TimeStamp { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
