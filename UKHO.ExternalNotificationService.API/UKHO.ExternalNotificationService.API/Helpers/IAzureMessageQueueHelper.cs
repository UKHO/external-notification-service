using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.API.Helper
{
    public interface IAzureMessageQueueHelper
    {
        Task AddQueueMessage<T>(string storageConnectionString, string queueName, T message);
    }
}
