using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public interface IHandleDeadLetterService
    {
        Task ProcessDeadLetter(string filePath, string subscriptionId, SubscriptionRequestMessage subscriptionRequestMessage, string fileName);
    }
}
