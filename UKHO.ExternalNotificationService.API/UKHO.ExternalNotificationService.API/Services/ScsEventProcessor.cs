using System;
using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using UKHO.ExternalNotificationService.Common.Models.Monitoring;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class ScsEventProcessor : EventProcessorBase, IEventProcessor
    {
        private readonly IScsEventValidationAndMappingService _scsEventValidationAndMappingService;
        private readonly ILogger<ScsEventProcessor> _logger;
        private readonly IOptions<EventProcessorConfiguration> _eventProcessorConfiguration;
        private readonly bool _addsMonitoringEnabled;
        private readonly IAddsMonitoringService _addsElasticMonitoringService;


        private List<Error> _errors = [];

        public string EventType => EventProcessorTypes.SCS;

        private const int MinDelay = 0;
        private const int MaxDelay = 10;
        public int DelayInMilliseconds => Math.Clamp(_eventProcessorConfiguration.Value.ScsEventPublishDelayInSeconds, MinDelay, MaxDelay) * 1000;
        
        public ScsEventProcessor(IScsEventValidationAndMappingService scsEventValidationAndMappingService,
                                ILogger<ScsEventProcessor> logger,
                                IAzureEventGridDomainService azureEventGridDomainService,
                                IOptions<EventProcessorConfiguration> eventProcessorConfiguration,
                                IConfiguration configuration,
                                IAddsMonitoringService addsElasticMonitoringService)
                               : base(azureEventGridDomainService)
        {
            _scsEventValidationAndMappingService = scsEventValidationAndMappingService;
            _logger = logger;
            _eventProcessorConfiguration = eventProcessorConfiguration;
            _addsElasticMonitoringService = addsElasticMonitoringService;

            _addsMonitoringEnabled = configuration.GetValue<bool>("ADDSMonitoringEnabled");
        }


        public async Task<ExternalNotificationServiceProcessResponse> Process(CustomCloudEvent customCloudEvent, string correlationId, CancellationToken cancellationToken = default)
        {
            CloudEventCandidate<ScsEventData> candidate = ConvertToCloudEventCandidate<ScsEventData>(customCloudEvent);

            ValidationResult validationScsEventData = await _scsEventValidationAndMappingService.ValidateScsEventData(candidate.Data!);

            if (!validationScsEventData.IsValid && validationScsEventData.HasOkErrors(out _errors))
                return ProcessResponse();

            _logger.LogInformation(EventIds.ScsEventDataMappingStart.ToEventId(), "Sales catalogue service event data mapping started for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);
            CloudEvent cloudEvent = _scsEventValidationAndMappingService.MapToCloudEvent(candidate);
            _logger.LogInformation(EventIds.ScsEventDataMappingCompleted.ToEventId(), "Sales catalogue service event data mapping successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, correlationId);

            await PublishEventWithDelayAsync(cloudEvent, correlationId, DelayInMilliseconds, cancellationToken);

            if (_addsMonitoringEnabled)
            {
                await _addsElasticMonitoringService.StopProcessAsync(new AddsData
                {
                    EditionNumber = candidate.Data?.EditionNumber ?? 0,
                    ProductName = candidate.Data?.ProductName ?? string.Empty,
                    UpdateNumber = candidate.Data?.UpdateNumber ?? 0,
                    StatusName = candidate.Data?.Status.StatusName ?? string.Empty,
                    Type = ScsDataMappingValueConstant.Type
                }, correlationId, cancellationToken);
            }

            return ProcessResponse();
        }

        private ExternalNotificationServiceProcessResponse ProcessResponse()
        {
            return new ExternalNotificationServiceProcessResponse() { Errors = _errors, StatusCode = HttpStatusCode.OK };
        }
    }
}
