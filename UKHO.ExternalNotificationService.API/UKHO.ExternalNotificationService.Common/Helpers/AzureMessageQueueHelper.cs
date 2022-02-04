
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureMessageQueueHelper : IAzureMessageQueueHelper
    {
        public async Task<HealthCheckResult> CheckMessageQueueHealth(string storageAccountConnectionString, string queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            var queueMessageExists = await queue.ExistsAsync();
            if (queueMessageExists)
                return HealthCheckResult.Healthy("Azure message queue is healthy");
            else
                return HealthCheckResult.Unhealthy("Azure message queue is unhealthy", new Exception($"Azure message queue {queueName} does not exists"));
        }
    }
}
