
namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class AzureConfiguration
    {
        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public string EventGridDomainName { get; set; }

        public string EventGridDomainAccessKey { get; set; }

        public string EventGridDomainEndpoint { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageConnectionString { get; set; }

        public string SubscriptionRequestQueueName { get; set; }

        public string SubscriptionCallbackQueueName { get; set; }
    }
}
