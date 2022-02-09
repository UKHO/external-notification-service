using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class EventGridDomainConfiguration
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
