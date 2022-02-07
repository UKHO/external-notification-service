
namespace UKHO.ExternalNotificationService.Common.Storage
{
    public interface ISubscriptionStorageService
    {
        string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null);
    }
}
