using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    public interface IAzureMoveBlobHelper
    {
        Task DeadLetterMoveBlob(SubscriptionStorageConfiguration ensStorageConfiguration, string path);
    }
}
