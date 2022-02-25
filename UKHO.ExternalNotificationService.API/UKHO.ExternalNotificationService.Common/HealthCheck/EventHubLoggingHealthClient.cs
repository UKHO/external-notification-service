using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.Logging.EventHubLogProvider;

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
            var eventHubProducerClient = new EventHubProducerClient(_eventHubLoggingConfiguration.Value.ConnectionString, _eventHubLoggingConfiguration.Value.EntityPath);

            try
            {
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow,
                    Level = LogLevel.Trace.ToString(),
                    MessageTemplate = "Event Hub Logging Event Data For Health Check",
                    LogProperties = new Dictionary<string, object>
                    {
                        { "_Environment", _eventHubLoggingConfiguration.Value.Environment },
                        { "_System",      _eventHubLoggingConfiguration.Value.System },
                        { "_Service",     _eventHubLoggingConfiguration.Value.Service },
                        { "_NodeName",    _eventHubLoggingConfiguration.Value.NodeName }
                    },
                    EventId = EventIds.EventHubLoggingEventDataForHealthCheck.ToEventId()
                };

                string jsonLogEntry = JsonConvert.SerializeObject(logEntry);

                using EventDataBatch eventBatch = await eventHubProducerClient.CreateBatchAsync(cancellationToken);

                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonLogEntry)));

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
                await eventHubProducerClient.DisposeAsync();
            }
        }
    }
}
