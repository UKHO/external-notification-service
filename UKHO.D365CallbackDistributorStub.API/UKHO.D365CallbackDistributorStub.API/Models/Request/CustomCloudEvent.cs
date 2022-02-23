namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CustomCloudEvent
    {
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string DataContentType { get; set; }
        public Data Data { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attribute
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Get
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Get get { get; set; }
        public BatchDetails batchDetails { get; set; }
        public BatchStatus batchStatus { get; set; }
    }

    public class File
    {
        public string mimeType { get; set; }
        public List<object> attributes { get; set; }
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

    public class Data
    {
        public string batchId { get; set; }
        public List<Attribute> attributes { get; set; }
        public DateTime batchPublishedDate { get; set; }
        public List<File> files { get; set; }
        public Links links { get; set; }
        public string businessUnit { get; set; }
    }

    
}
