namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class DistributorRequest
    {
        public CustomCloudEvent? cloudEvent {get;set;}
        public string? cloudEventId { get; set; }
        public Guid guid { get; set; }
    }
}
