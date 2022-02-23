using Azure.Messaging;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class DistributorRequest
    {
        public CustomCloudEvent? cloudEvent {get;set;}
        public string cloudEventId { get; set; }
        public Guid guid { get; set; }
    }
}
