using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class EnsSubscriptionStorageConfiguration
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageContainerName { get; set; }
        public string QueueName { get; set; }
    }
}
