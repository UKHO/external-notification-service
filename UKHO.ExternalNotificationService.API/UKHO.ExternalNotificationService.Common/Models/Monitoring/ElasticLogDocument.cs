using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.Monitoring;

public class ElasticLogDocument
{
    [JsonPropertyName("@timestamp")]
    public DateTime Timestamp { get; set; }
    public string Id { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int? EditionNumber { get; set; }
    public int UpdateNumber { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusDate { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime? StartTimestamp { get; set; }
    public DateTime? StopTimestamp { get; set; }
    public int? Duration { get; set; }
    public bool? ImmediateRelease { get; set; }
    public string Environment { get; set; } = string.Empty;
    public bool IsAbnormal { get; set; }
    public bool IsComplete { get; set; }
}
