
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class SubscriptionStorageConfiguration
    {
        public string StorageAccountName { get; set; } = string.Empty;
        public string StorageAccountKey { get; set; } = string.Empty;       
        public string StorageContainerName { get; set; } = string.Empty;
        public string DeadLetterDestinationContainerName { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
