using System.Net;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class DistributorRequest
    {
        public CustomCloudEvent? CloudEvent { get; set; }
        public string? Subject { get; set; }
        public Guid Guid { get; set; }
        public string? TimeStamp { get; set; }
        public HttpStatusCode? statusCode { get; set; }
    }
}
