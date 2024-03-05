using System.Net;
using Azure.Messaging;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class DistributorRequest
    {
        public CloudEvent? CloudEvent { get; set; }
        public string? Subject { get; set; }
        public Guid Guid { get; set; }
        public DateTime? TimeStamp { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
    }
}
