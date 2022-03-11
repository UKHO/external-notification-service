
namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class CustomCloudEvent 
    {
        public string Type { get; set; }
        public string Time { get; set; }
        public string DataContentType { get; set; }
        public string DataSchema { get; set; }
        public string Subject { get; set; }
        public string Source { get; set; }
        public string Id { get; set; }
        public object Data { get; set; }
    }
}
