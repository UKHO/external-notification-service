using System;
using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class BatchAttribute
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class GetUrl
    {
        public string href { get; set; }
    }

    public class Links
    {
        public BatchDetails batchDetails { get; set; }
        public BatchStatus batchStatus { get; set; }
        public GetUrl getUrl { get; set; }
    }

    public class BatchFile
    {
        public string mimeType { get; set; }
        public List<BatchAttribute> attributes { get; set; }
        public string hash { get; set; }
        public Links links { get; set; }
        public string filename { get; set; }
        public int fileSize { get; set; }
    }

    public class BatchDetails
    {
        public string href { get; set; }
    }

    public class BatchStatus
    {
        public string href { get; set; }
    }

    public class EnterpriseEventServiceDataRequest
    {
        public string batchId { get; set; }
        public List<BatchAttribute> batchAttributes { get; set; }
        public DateTime batchPublishedDate { get; set; }
        public List<BatchFile> files { get; set; }
        public Links links { get; set; }
        public string businessUnit { get; set; }
    }


}
