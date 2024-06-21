using System;
using System.Net;
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class DistributorRequest
    {
        [JsonPropertyName(name: "cloudEvent")]
        public CustomCloudEvent CloudEvent { get; set; }
        [JsonPropertyName(name: "subject")]
        public string Subject { get; set; }
        [JsonPropertyName(name: "guid")]
        public Guid Guid { get; set; }
        [JsonPropertyName(name: "timeStamp")]
        public DateTime TimeStamp { get; set; }
        [JsonPropertyName(name: "statusCode")]
        public HttpStatusCode? StatusCode { get; set; }
    }
}
