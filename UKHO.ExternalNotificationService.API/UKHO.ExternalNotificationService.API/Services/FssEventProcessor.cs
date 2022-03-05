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
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class FssEventProcessor : IEventProcessor
    {
        private readonly IFssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private readonly IOptions<FssDataMappingConfiguration> _fssDataMappingConfiguration;
        private readonly ILogger<FssEventProcessor> _logger;
        private List<Error> _errors;
        private readonly IAzureEventGridDomainService _azureEventGridDomainService;

        public string EventType => EventProcessorTypes.FSS;

        public FssEventProcessor(IFssEventValidationAndMappingService fssEventValidationAndMappingService,
                                 IOptions<FssDataMappingConfiguration> fssDataMappingConfiguration,
                                 ILogger<FssEventProcessor> logger,
                                 IAzureEventGridDomainService azureEventGridDomainService)
        {
            _fssEventValidationAndMappingService = fssEventValidationAndMappingService;
            _fssDataMappingConfiguration = fssDataMappingConfiguration;
            _logger = logger;
            _azureEventGridDomainService = azureEventGridDomainService;
        }

        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomEventGridEvent customEventGridEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            string data =JsonConvert.SerializeObject(customEventGridEvent.Data);
            FssEventData fssEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            ValidationResult validationFssEventData = await _fssEventValidationAndMappingService.ValidateFssEventData(fssEventData);

            if (!validationFssEventData.IsValid && validationFssEventData.HasOkErrors(out _errors))
                return ReturnProcessResponse(fssEventData);

            if (fssEventData.BusinessUnit == _fssDataMappingConfiguration.Value.BusinessUnit)
            {
                _logger.LogInformation(EventIds.FssEventDataMappingStart.ToEventId(), "File share service event data mapping started for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, fssEventData.BusinessUnit, correlationId);
                CloudEvent cloudEvent = _fssEventValidationAndMappingService.FssEventDataMapping(customEventGridEvent, correlationId);
                _logger.LogInformation(EventIds.FssEventDataMappingCompleted.ToEventId(), "File share service event data mapping completed for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, fssEventData.BusinessUnit, correlationId);

                await _azureEventGridDomainService.PublishEventAsync(cloudEvent, correlationId, cancellationToken);
            }
            else
            {
                _logger.LogInformation(EventIds.FssEventDataWithInvalidBusinessUnit.ToEventId(), "External notification service webhook request is failed due to invalid business unit for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, fssEventData.BusinessUnit, correlationId);
            }

            return ReturnProcessResponse(fssEventData);
        }

        private ExternalNotificationServiceProcessResponse ReturnProcessResponse(FssEventData fssEventData)
        {
            return new ExternalNotificationServiceProcessResponse() { BusinessUnit = fssEventData.BusinessUnit, Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
