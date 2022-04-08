
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class SubscriptionStorageConfiguration
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }       
        public string StorageContainerName { get; set; }
        public string DeadLetterDestinationContainerName { get; set; }
        public string QueueName { get; set; }
    }
}
