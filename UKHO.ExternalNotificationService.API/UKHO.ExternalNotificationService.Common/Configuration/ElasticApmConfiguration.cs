namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class ElasticApmConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string CloudId { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string AddsApiKey { get; set; } = string.Empty;
    }
}
