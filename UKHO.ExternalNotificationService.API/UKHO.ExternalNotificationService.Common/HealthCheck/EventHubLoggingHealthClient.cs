using Microsoft.Azure.EventHubs;
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
            this._eventHubLoggingConfiguration = eventHubLoggingConfiguration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_eventHubLoggingConfiguration.Value.ConnectionString)
            {
                EntityPath = _eventHubLoggingConfiguration.Value.EntityPath
            };

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            try
            {
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(EventIds.EventHubLoggingEventDataForHealthCheck.ToEventId() + " of Event Hub")));
                return HealthCheckResult.Healthy("Event hub is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Event hub is unhealthy", new Exception(ex.Message));
            }
            finally
            {
                await eventHubClient.CloseAsync();
            }
        }
    }
}
