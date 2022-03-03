using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Extensions;
using UKHO.ExternalNotificationService.Common.BaseClass;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class FssEventProcessor : EventProcessorBase, IEventProcessor
    {
        private readonly IFssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private readonly IOptions<FssDataMappingConfiguration> _fssDataMappingConfiguration;
        private readonly ILogger<FssEventProcessor> _logger;
        private List<Error> _errors;

        public string EventType => EventProcessorTypes.FSS;
        

        public FssEventProcessor(IOptions<EventGridDomainConfiguration> eventGridDomainConfig,
                                 IFssEventValidationAndMappingService fssEventValidationAndMappingService,
                                 IOptions<FssDataMappingConfiguration> fssDataMappingConfiguration,
                                 ILogger<FssEventProcessor> logger)
            : base(eventGridDomainConfig, logger)
        {
            _fssEventValidationAndMappingService = fssEventValidationAndMappingService;
            _fssDataMappingConfiguration = fssDataMappingConfiguration;
            _logger = logger;
        }

        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomEventGridEvent customEventGridEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            string data =JsonConvert.SerializeObject(customEventGridEvent.Data);
            FssEventData fssEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            ValidationResult validationFssEventData = await _fssEventValidationAndMappingService.ValidateFssEventData(fssEventData);

            if (!validationFssEventData.IsValid && validationFssEventData.HasBadRequestErrors(out _errors))
            {
                return new ExternalNotificationServiceProcessResponse() { BusinessUnit = fssEventData.BusinessUnit, Errors = _errors, StatusCode = HttpStatusCode.OK};
            }

            if (fssEventData.BusinessUnit == _fssDataMappingConfiguration.Value.BusinessUnit)
            {
                _logger.LogInformation(EventIds.FssEventDataMappingStart.ToEventId(), "Fss event data mapping started for subject:{subject}, BusinessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, fssEventData.BusinessUnit, correlationId);
                CloudEvent cloudEvent = _fssEventValidationAndMappingService.FssEventDataMapping(customEventGridEvent, correlationId);
                _logger.LogInformation(EventIds.FssEventDataMappingCompleted.ToEventId(), "Fss event data mapping completed for subject:{subject}, BusinessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, fssEventData.BusinessUnit, correlationId);

                await PublishEventAsync(cloudEvent, correlationId, cancellationToken);
            }

            return new ExternalNotificationServiceProcessResponse() { Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
