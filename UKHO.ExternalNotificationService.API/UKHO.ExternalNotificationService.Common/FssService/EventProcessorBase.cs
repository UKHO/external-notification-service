using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.Common.FssService
{
    public class EventProcessorBase
    {
        protected readonly IOptions<EventGridDomainConfiguration> _eventGridDomainConfig;
        private readonly ILogger<EventProcessorBase> _logger;

        public EventProcessorBase(IOptions<EventGridDomainConfiguration> eventGridDomainConfig,
                                  ILogger<EventProcessorBase> logger)
        {
            _eventGridDomainConfig = eventGridDomainConfig;
            _logger = logger;
        }

        protected async Task<bool> PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(EventIds.ENSEventPublishStart.ToEventId(), "External notification service event publish started for subject:{subject} and _X-Correlation-ID:{correlationId}.", cloudEvent.Subject, correlationId);
            
            EventGridPublisherClient client = new(new Uri(_eventGridDomainConfig.Value.EventGridDomainEndpoint),
                                                  new AzureKeyCredential(_eventGridDomainConfig.Value.EventGridDomainAccessKey));

            List<CloudEvent> listCloudEvent = new() { cloudEvent };

            try
            {
                await client.SendEventsAsync(listCloudEvent, cancellationToken);

                _logger.LogInformation(EventIds.ENSEventPublishCompleted.ToEventId(), "External notification service event publish completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", cloudEvent.Subject, correlationId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ENSEventNotPublished.ToEventId(), ex, "External notification service event not published for subject:{subject} and _X-Correlation-ID:{correlationId} with error {Message}.", cloudEvent.Subject, correlationId, ex.Message);

                return false;
            }
        }
    }
}
