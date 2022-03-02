
namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IWebhookService
    {
        IEventProcessor GetProcessor(string eventType);
    }
}
