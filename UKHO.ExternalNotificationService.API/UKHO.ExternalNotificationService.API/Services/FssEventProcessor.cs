using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Extensions;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.FssService;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class FssEventProcessor : EventProcessorBase, IEventProcessor
    {
        private readonly IFssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private readonly IOptions<FssDataMappingConfiguration> _fssDataMappingConfiguration;
        private List<Error> _errors;

        public string EventType => EventProcessorTypes.FSS;
        

        public FssEventProcessor(IOptions<EventGridDomainConfiguration> eventGridDomainConfig,
                                 IFssEventValidationAndMappingService fssEventValidationAndMappingService,
                                 IOptions<FssDataMappingConfiguration> fssDataMappingConfiguration)
            : base(eventGridDomainConfig)
        {
            _fssEventValidationAndMappingService = fssEventValidationAndMappingService;
            _fssDataMappingConfiguration = fssDataMappingConfiguration;
        }

        public async Task<IActionResult> Process(CustomEventGridEvent customEventGridEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            string data =JsonConvert.SerializeObject(customEventGridEvent.Data);
            FssEventData fssEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            ValidationResult validationFssEventData = await _fssEventValidationAndMappingService.ValidateFssEventData(fssEventData);

            if (!validationFssEventData.IsValid && validationFssEventData.HasBadRequestErrors(out _errors))
            {
                return new BadRequestObjectResult(new ErrorDescription
                {
                    Errors = _errors,
                    CorrelationId = correlationId
                });
            }

            if (fssEventData.BusinessUnit == _fssDataMappingConfiguration.Value.BusinessUnit)
            {
                CloudEvent cloudEvent = _fssEventValidationAndMappingService.FssEventDataMapping(customEventGridEvent, correlationId);

                await PublishEventAsync(cloudEvent, correlationId, cancellationToken);
            }

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}
