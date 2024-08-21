namespace UKHO.ExternalNotificationService.Common.Models.Monitoring
{
    public class AddsData
    {
        public string Type { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int EditionNumber { get; set; }
        public int UpdateNumber { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
