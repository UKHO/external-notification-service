using Microsoft.Azure.EventGrid.Models;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CustomCloudEvent : EventGridEvent
    {
        public string? Type { get; set; }
        public string? Time { get; set; }
        public string? DataContentType { get; set; }
        public string? DataSchema { get; set; }
        public string? Source { get; set; }
    }
}
