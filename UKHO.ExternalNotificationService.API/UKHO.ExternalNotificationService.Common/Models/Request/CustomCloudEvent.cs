
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    
    public class CustomCloudEvent
    {
        [JsonPropertyName(name: "id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName(name: "type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName(name: "time")]
        public string Time { get; set; } = string.Empty;
        [JsonPropertyName(name: "dataContentType")]
        public string DataContentType { get; set; } = string.Empty;
        [JsonPropertyName(name: "dataSchema")]
        public string DataSchema { get; set; } = string.Empty;
        [JsonPropertyName(name: "subject")]
        public string Subject { get; set; } = string.Empty;
        [JsonPropertyName(name: "source")]
        public string Source { get; set; } = string.Empty;
        [JsonPropertyName(name: "data")]
        public object? Data { get; set; }
    }
}
