using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Extensions;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class ScsS100EventValidationAndMappingService : IScsS100EventValidationAndMappingService
    {
        private readonly IScsEventDataValidator _scsEventDataValidator;
        private readonly IOptions<ScsS100DataMappingConfiguration> _scsS100DataMappingConfiguration;

        public ScsS100EventValidationAndMappingService(IScsEventDataValidator scsEventDataValidator,
                                                   IOptions<ScsS100DataMappingConfiguration> scsS100DataMappingConfiguration)
        {
            _scsEventDataValidator = scsEventDataValidator;
            _scsS100DataMappingConfiguration = scsS100DataMappingConfiguration;
        }

        public Task<ValidationResult> ValidateScsEventData(ScsEventData scsEventData)
        {
            return _scsEventDataValidator.Validate(scsEventData);
        }

        
        public CloudEvent MapToCloudEvent(CloudEventCandidate<ScsEventData> candidate)
        {
            CloudEvent cloudEvent = new(_scsS100DataMappingConfiguration.Value.Source,
                                        ScsS100DataMappingValueConstant.Type,
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
