using System.Net;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.API.Extensions;
using UKHO.ExternalNotificationService.Common.BaseClass;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class ScsS100EventProcessor : EventProcessorBase, IEventProcessor
    {
        private readonly IScsS100EventValidationAndMappingService _scsS100EventValidationAndMappingService;
        private readonly ILogger<ScsS100EventProcessor> _logger;
        private readonly IOptions<EventProcessorConfiguration> _eventProcessorConfiguration;
        
        private List<Error> _errors = [];

        public string EventType => EventProcessorTypes.SCSS100;

        private const int MinDelay = 0;
        private const int MaxDelay = 10;
        public int DelayInMilliseconds => Math.Clamp(_eventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds, MinDelay, MaxDelay) * 1000;
        
        public ScsS100EventProcessor(IScsS100EventValidationAndMappingService scsS100EventValidationAndMappingService,
                                ILogger<ScsS100EventProcessor> logger,
                                IAzureEventGridDomainService azureEventGridDomainService,
                                IOptions<EventProcessorConfiguration> eventProcessorConfiguration)
                               : base(azureEventGridDomainService)
        {
            _scsS100EventValidationAndMappingService = scsS100EventValidationAndMappingService;
            _logger = logger;
            _eventProcessorConfiguration = eventProcessorConfiguration;
        }


        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomCloudEvent customCloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            var candidate = ConvertToCloudEventCandidate<ScsEventData>(customCloudEvent);

            var validationScsEventData = await _scsS100EventValidationAndMappingService.ValidateScsEventData(candidate.Data!);

            if (!validationScsEventData.IsValid && validationScsEventData.HasOkErrors(out _errors))
                return ProcessResponse();

            _logger.LogInformation(EventIds.ScsS100EventDataMappingStart.ToEventId(), "Sales catalogue service event data mapping started for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);
            var cloudEvent = _scsS100EventValidationAndMappingService.MapToCloudEvent(candidate);
            _logger.LogInformation(EventIds.ScsS100EventDataMappingCompleted.ToEventId(), "Sales catalogue service event data mapping successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);

            await PublishEventWithDelayAsync(cloudEvent, correlationId, DelayInMilliseconds, cancellationToken);
            
            return ProcessResponse();
        }

        private ExternalNotificationServiceProcessResponse ProcessResponse()
        {
            return new ExternalNotificationServiceProcessResponse() { Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
