using System;
using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.Common.Models.EventModel
{
    public class FssEventData
    {
        public string BatchId { get; set; }
        public BatchLinks Links { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public string BusinessUnit { get; set; }
        public DateTime? BatchPublishedDate { get; set; }
        public IEnumerable<File> Files { get; set; }
    }

    public class Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class BatchLinks
    {
        public Link BatchDetails { get; set; }
        public Link BatchStatus { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }

    public class File
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string MIMEType { get; set; }
        public string Hash { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public FileLinks Links { get; set; }
    }

    public class FileLinks
    {
        public Link Get { get; set; }
    }
}
