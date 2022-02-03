
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class EventHubLoggingHealthClient : IEventHubLoggingHealthClient
    {
        private readonly IOptions<EventHubLoggingConfiguration> _eventHubLoggingConfiguration;

        public EventHubLoggingHealthClient(IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {
            _eventHubLoggingConfiguration = eventHubLoggingConfiguration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var eventHubProducerClient = new EventHubProducerClient(_eventHubLoggingConfiguration.Value.ConnectionString, _eventHubLoggingConfiguration.Value.EntityPath);
                using EventDataBatch eventBatch = await eventHubProducerClient.CreateBatchAsync(cancellationToken);
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(EventIds.EventHubLoggingEventDataForHealthCheck.ToEventId() + " of Event Hub")));
                try
                {
                    await eventHubProducerClient.SendAsync(eventBatch, cancellationToken);
                    return HealthCheckResult.Healthy("Event hub is healthy");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Event hub is unhealthy", new Exception(ex.Message));
                }
                finally
                {
                    await eventHubProducerClient.CloseAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Event hub is unhealthy", new Exception(ex.Message));
            }
        }
    }
}
