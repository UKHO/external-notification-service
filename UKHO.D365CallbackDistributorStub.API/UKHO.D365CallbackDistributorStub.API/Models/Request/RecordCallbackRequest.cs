using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class RecordCallbackRequest
    {
        public CallbackRequest? CallBackRequest { get; set; }
        public string? SubscriptionId { get; set; }
        public Guid Guid { get; set; }
        public DateTime? TimeStamp { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
