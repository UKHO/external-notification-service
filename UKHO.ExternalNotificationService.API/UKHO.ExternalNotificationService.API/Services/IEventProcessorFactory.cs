
namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IEventProcessorFactory
    {
        IEventProcessor? GetProcessor(string eventType);
    }
}
