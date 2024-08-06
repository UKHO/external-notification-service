using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class CustomCloudEvent
    {
        [JsonPropertyName(name: "id")]
        public string Id { get; set; }
        [JsonPropertyName(name: "type")]
        public string Type { get; set; }
        [JsonPropertyName(name: "time")]
        public string Time { get; set; }
        [JsonPropertyName(name: "dataContentType")]
        public string DataContentType { get; set; }
        [JsonPropertyName(name: "dataSchema")]
        public string DataSchema { get; set; }
        [JsonPropertyName(name: "subject")]
        public string Subject { get; set; }
        [JsonPropertyName(name: "source")]
        public string Source { get; set; }
        [JsonPropertyName(name: "data")]
        public object Data { get; set; }
    }

}
