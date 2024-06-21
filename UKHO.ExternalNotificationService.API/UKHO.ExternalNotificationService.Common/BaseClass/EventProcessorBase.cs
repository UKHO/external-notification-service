using Azure.Messaging;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;

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

        /// <summary>
        /// Converts the provided CustomCloudEvent to a CloudEventCandidate of type T.
        /// CloudEventCandidate contains the properties needed to create a CloudEvent.
        /// This includes the appropriately typed Data property.
        /// </summary>
        /// <typeparam name="T">The type of the CloudEventCandidate.</typeparam>
        /// <param name="source">The CustomCloudEvent to convert.</param>
        /// <returns>The converted CloudEventCandidate of type T.</returns>
        public CloudEventCandidate<T> ConvertToCloudEventCandidate<T>(CustomCloudEvent source) where T : class
        {
            CloudEventCandidate<T> cloudEventCandidate = new CloudEventCandidate<T>
            {
                DataContentType = source.DataContentType,
                DataSchema = source.DataSchema,
                Subject = source.Subject
            };
            cloudEventCandidate.Data = GetEventData<T>(source.Data!);
            return cloudEventCandidate;
        }

        public async Task PublishEventAsync(CloudEvent cloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            await _azureEventGridDomainService.PublishEventAsync(cloudEvent, correlationId, cancellationToken);
        }
    }
}
