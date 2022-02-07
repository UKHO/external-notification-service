using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public void ProcessQueueMessage([QueueTrigger("%SubscriptionStorageConfiguration:QueueName%")] QueueMessage message)
        {
            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "check web job is triggered or not using Azure Queue {message}", message.Body.ToString());
            System.Console.WriteLine(message);
        }
    }
}
