using Azure.Messaging;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    public interface IDistributionWebhookService
    {
        CloudEvent TryGetCloudEventMessage(string jsonContent);
    }
}
