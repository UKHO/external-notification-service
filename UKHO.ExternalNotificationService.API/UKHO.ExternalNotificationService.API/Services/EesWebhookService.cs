using System;
using Azure.Messaging;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class EesWebhookService : IEesWebhookService
    {
        public CloudEvent TryGetCloudEventMessage(string jsonContent)
        {
            try
            {
                CloudEvent cloudEvent = CloudEvent.Parse(BinaryData.FromString(jsonContent),true);
                return cloudEvent;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
