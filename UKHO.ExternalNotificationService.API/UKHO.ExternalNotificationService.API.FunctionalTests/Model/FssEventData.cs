using System;
using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
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
}
