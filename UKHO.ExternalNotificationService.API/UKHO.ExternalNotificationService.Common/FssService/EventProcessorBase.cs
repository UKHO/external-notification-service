using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.FssService
{
    public class EventProcessorBase
    {
        protected readonly IOptions<EventGridDomainConfiguration> _eventGridDomainConfig;

        public EventProcessorBase(IOptions<EventGridDomainConfiguration> eventGridDomainConfig)
        {
            _eventGridDomainConfig = eventGridDomainConfig;
        }

        protected async Task<bool> PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            EventGridPublisherClient client = new(new Uri(_eventGridDomainConfig.Value.EventGridDomainEndpoint),
                                                  new AzureKeyCredential(_eventGridDomainConfig.Value.EventGridDomainAccessKey));

            List<CloudEvent> listCloudEvent = new() { cloudEvent };

            try
            {
                await client.SendEventsAsync(listCloudEvent, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
