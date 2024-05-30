using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Extensions;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class ScsEventValidationAndMappingService : IScsEventValidationAndMappingService
    {
        private readonly IScsEventDataValidator _scsEventDataValidator;
        private readonly IOptions<ScsDataMappingConfiguration> _scsDataMappingConfiguration;

        public ScsEventValidationAndMappingService(IScsEventDataValidator scsEventDataValidator,
                                                   IOptions<ScsDataMappingConfiguration> scsDataMappingConfiguration)
        {
            _scsEventDataValidator = scsEventDataValidator;
            _scsDataMappingConfiguration = scsDataMappingConfiguration;
        }

        public Task<ValidationResult> ValidateScsEventData(ScsEventData scsEventData)
        {
            return _scsEventDataValidator.Validate(scsEventData);
        }

        
        public CloudEvent MapToCloudEvent(CloudEventCandidate<ScsEventData> candidate)
        {
            CloudEvent cloudEvent = new(_scsDataMappingConfiguration.Value.Source,
                                        ScsDataMappingValueConstant.Type,
                                        candidate.Data)
            {
                Time = DateTimeOffset.Parse(DateTime.UtcNow.ToRfc3339String()),
                Id = Guid.NewGuid().ToString(),
                Subject = candidate.Subject,
                DataContentType = candidate.DataContentType,
                DataSchema = candidate.DataSchema
            };

            return cloudEvent;
        }
    }
}
