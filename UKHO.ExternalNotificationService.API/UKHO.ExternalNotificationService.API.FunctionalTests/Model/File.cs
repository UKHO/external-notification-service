using System;
using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class File
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string MIMEType { get; set; }
        public string Hash { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public FileLinks Links { get; set; }
    }
}
