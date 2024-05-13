﻿using Azure.Messaging;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Helpers;

namespace UKHO.ExternalNotificationService.Common.BaseClass
{
    public class EventProcessorBase
    {
        private readonly IAzureEventGridDomainService _azureEventGridDomainService;

        public EventProcessorBase(IAzureEventGridDomainService azureEventGridDomainService)
        {
            _azureEventGridDomainService = azureEventGridDomainService;
        }

        public T? GetEventData<T>(object data) where T : class 
        {
            return _azureEventGridDomainService.ConvertObjectTo<T>(data);
        }

        public async Task PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            await _azureEventGridDomainService.PublishEventAsync(cloudEvent, correlationId, cancellationToken);
        }
    }
}
