using Azure.Messaging;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IEesWebhookService
    {
        CloudEvent TryGetCloudEventMessage(string jsonContent);
    }
}
