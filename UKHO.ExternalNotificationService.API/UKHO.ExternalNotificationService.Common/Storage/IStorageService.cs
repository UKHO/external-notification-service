
namespace UKHO.ExternalNotificationService.Common.Storage
{
    public interface IStorageService
    {
        string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null);
    }
}
