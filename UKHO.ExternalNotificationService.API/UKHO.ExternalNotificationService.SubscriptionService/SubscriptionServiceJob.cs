using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public class SubscriptionServiceJob
    {
        private readonly IConfiguration _confugration;
        private readonly ILogger<SubscriptionServiceJob> _logger;

        public SubscriptionServiceJob(IConfiguration confugration, ILogger<SubscriptionServiceJob> logger)
        {
            _confugration = confugration;
            _logger = logger;
        }

        public void ProcessQueueMessage([QueueTrigger("%EnsSubscriptionStorageConfiguration:QueueName%")] string message)
        {
            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "check web job is triggered or not using Azure Queue {message}", message);
            System.Console.WriteLine(message);
        }
    }
}
