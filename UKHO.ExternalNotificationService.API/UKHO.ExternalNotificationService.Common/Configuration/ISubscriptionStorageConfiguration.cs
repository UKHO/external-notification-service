
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public interface ISubscriptionStorageConfiguration
    {
         string StorageAccountName { get; set; }
         string StorageAccountKey { get; set; }
         string StorageContainerName { get; set; }
         string QueueName { get; set; }

    }
}
