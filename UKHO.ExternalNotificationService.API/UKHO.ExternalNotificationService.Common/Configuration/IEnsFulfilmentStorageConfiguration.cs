
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public interface IEnsFulfilmentStorageConfiguration
    {
         string StorageAccountName { get; set; }
         string StorageAccountKey { get; set; }
         string StorageContainerName { get; set; }
       
    }
}
