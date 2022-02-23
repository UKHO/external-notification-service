using Azure.Messaging;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    public class DistributionWebhookService : IDistributionWebhookService
    {
        public CloudEvent TryGetCloudEventMessage(string jsonContent)
        {
            try
            {
                CloudEvent? cloudEvent = CloudEvent.Parse(BinaryData.FromString(jsonContent),true);
                return cloudEvent;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
