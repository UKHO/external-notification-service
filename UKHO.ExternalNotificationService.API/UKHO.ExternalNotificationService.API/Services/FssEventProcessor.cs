using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
    public class FssEventProcessor : EventProcessorBase , IEventProcessor
    {
        private readonly IFssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private readonly ILogger<FssEventProcessor> _logger;
        private List<Error> _errors;
        private readonly IAzureEventGridDomainService _azureEventGridDomainService;

        public string EventType => EventProcessorTypes.FSS;

        public FssEventProcessor(IFssEventValidationAndMappingService fssEventValidationAndMappingService,
                                 ILogger<FssEventProcessor> logger,
                                 IAzureEventGridDomainService azureEventGridDomainService)
                                : base(azureEventGridDomainService)
        {
            _fssEventValidationAndMappingService = fssEventValidationAndMappingService;
            _logger = logger;
            _azureEventGridDomainService = azureEventGridDomainService;
        }

        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomCloudEvent customCloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            FssEventData fssEventData = GetEventData<FssEventData>(customCloudEvent.Data);

            ValidationResult validationFssEventData = await _fssEventValidationAndMappingService.ValidateFssEventData(fssEventData);

            if (!validationFssEventData.IsValid && validationFssEventData.HasOkErrors(out _errors))
                return ProcessResponse(fssEventData);

            if (!string.IsNullOrWhiteSpace(BusinessUnitTypes.BusinessUnit.FirstOrDefault(x => x.Equals(fssEventData.BusinessUnit))))
            {
                _logger.LogInformation(EventIds.FssEventDataMappingStart.ToEventId(), "File share service event data mapping started for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, fssEventData.BusinessUnit, correlationId);
                CloudEvent cloudEvent = _fssEventValidationAndMappingService.FssEventDataMapping(customCloudEvent, correlationId);
                _logger.LogInformation(EventIds.FssEventDataMappingCompleted.ToEventId(), "File share service event data mapping successfully completed for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, fssEventData.BusinessUnit, correlationId);

                await PublishEventAsync(cloudEvent, correlationId, cancellationToken);
            }
            else
            {
                _errors = new List<Error>{ new Error(){  Source = "businessUnit",
                                                         Description = "Invalid business unit in an event."}};

                _logger.LogInformation(EventIds.FssEventDataWithInvalidBusinessUnit.ToEventId(), "External notification service webhook request failed due to an invalid business unit for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, fssEventData.BusinessUnit, correlationId);
            }

            return ProcessResponse(fssEventData);
        }

        private ExternalNotificationServiceProcessResponse ProcessResponse(FssEventData fssEventData)
        {
            return new ExternalNotificationServiceProcessResponse() { BusinessUnit = fssEventData.BusinessUnit, Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
