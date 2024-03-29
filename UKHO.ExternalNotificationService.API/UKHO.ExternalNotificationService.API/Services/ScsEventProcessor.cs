﻿using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
    public class ScsEventProcessor : EventProcessorBase, IEventProcessor
    {
        private readonly IScsEventValidationAndMappingService _scsEventValidationAndMappingService;
        private readonly ILogger<ScsEventProcessor> _logger;
        private List<Error> _errors;

        public string EventType => EventProcessorTypes.SCS;

        public ScsEventProcessor(IScsEventValidationAndMappingService scsEventValidationAndMappingService,
                                ILogger<ScsEventProcessor> logger,
                                IAzureEventGridDomainService azureEventGridDomainService)
                               : base(azureEventGridDomainService)
        {
            _scsEventValidationAndMappingService = scsEventValidationAndMappingService;
            _logger = logger;
        }


        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomCloudEvent customCloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            ScsEventData scsEventData = GetEventData<ScsEventData>(customCloudEvent.Data);

            ValidationResult validationScsEventData = await _scsEventValidationAndMappingService.ValidateScsEventData(scsEventData);

            if (!validationScsEventData.IsValid && validationScsEventData.HasOkErrors(out _errors))
                return ProcessResponse();

            _logger.LogInformation(EventIds.ScsEventDataMappingStart.ToEventId(), "Sales catalogue service event data mapping started for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);
            CloudEvent cloudEvent = _scsEventValidationAndMappingService.ScsEventDataMapping(customCloudEvent, correlationId);
            _logger.LogInformation(EventIds.ScsEventDataMappingCompleted.ToEventId(), "Sales catalogue service event data mapping successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);

            await PublishEventAsync(cloudEvent, correlationId, cancellationToken);

            return ProcessResponse();
        }

        private ExternalNotificationServiceProcessResponse ProcessResponse()
        {
            return new ExternalNotificationServiceProcessResponse() { Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
