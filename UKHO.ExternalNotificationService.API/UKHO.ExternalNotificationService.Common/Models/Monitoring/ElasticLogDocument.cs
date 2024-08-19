using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.Monitoring;

public class ElasticLogDocument
{
    [JsonPropertyName("@timestamp")]
    public DateTime Timestamp { get; set; }
    public string Id { get; set; }
    public string ProductName { get; set; }
    public int? EditionNumber { get; set; }
    public int UpdateNumber { get; set; }
    public string StatusName { get; set; }
    public string EventType { get; set; }
    public string TraceId { get; set; }
    public DateTime? StartTimestamp { get; set; }
    public DateTime? StopTimestamp { get; set; }
    public int? Duration { get; set; }
    public bool? ImmediateRelease { get; set; }
    public string Environment { get; set; }
    public bool IsAbnormal { get; set; }
    public bool IsComplete { get; set; }
}
