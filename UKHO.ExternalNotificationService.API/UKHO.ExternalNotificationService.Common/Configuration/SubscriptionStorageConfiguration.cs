
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class SubscriptionStorageConfiguration : ISubscriptionStorageConfiguration
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }       
        public string QueueName { get; set; }
        public string StorageContainerName { get; set; }
    }
}
