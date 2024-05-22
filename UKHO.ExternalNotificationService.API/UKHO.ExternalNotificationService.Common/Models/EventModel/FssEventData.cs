using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.EventModel
{
    public class FssEventData
    {
        [JsonPropertyName(name: "batchId")]
        public string BatchId { get; set; } = string.Empty;
        [JsonPropertyName(name: "links")]
        public BatchLinks? Links { get; set; }
        [JsonPropertyName(name: "attributes")]
        public IEnumerable<Attribute> Attributes { get; set; } = [];
        [JsonPropertyName(name: "businessUnit")]
        public string BusinessUnit { get; set; } = string.Empty;
        [JsonPropertyName(name: "batchPublishedDate")]
        public DateTime? BatchPublishedDate { get; set; }
        [JsonPropertyName(name: "files")]
        public IEnumerable<File> Files { get; set; } = [];
    }

    public class Attribute
    {
        [JsonPropertyName(name: "key")]
        public string Key { get; set; } = string.Empty;
        [JsonPropertyName(name: "value")]
        public string Value { get; set; } = string.Empty;
    }

    public class BatchLinks
    {
        [JsonPropertyName(name: "batchDetails")]
        public Link? BatchDetails { get; set; }
        [JsonPropertyName(name: "batchStatus")]
        public Link? BatchStatus { get; set; }
    }

    public class Link
    {
        [JsonPropertyName(name: "href")]
        public string Href { get; set; } = string.Empty;
    }

    public class File
    {
        [JsonPropertyName(name: "fileName")]
        public string FileName { get; set; } = string.Empty;
        [JsonPropertyName(name: "fileSize")]
        public long FileSize { get; set; }
        [JsonPropertyName(name: "mimeType")]
        public string MIMEType { get; set; } = string.Empty;
        [JsonPropertyName(name: "hash")]
        public string Hash { get; set; } = string.Empty;
        [JsonPropertyName(name: "attributes")]
        public IEnumerable<Attribute> Attributes { get; set; } = [];
        [JsonPropertyName(name: "links")]
        public FileLinks? Links { get; set; }
    }

    public class FileLinks
    {
        [JsonPropertyName(name: "get")]
        public Link? Get { get; set; }
    }
}
