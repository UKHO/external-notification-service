using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Exceptions;
using UKHO.ExternalNotificationService.Common.Extensions;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class FssEventValidationAndMappingService : IFssEventValidationAndMappingService
    {
        private readonly IFssEventDataValidator _fssEventDataValidator;
        private readonly IOptions<FssDataMappingConfiguration> _fssDataMappingConfiguration;

        public FssEventValidationAndMappingService(IFssEventDataValidator fssEventDataValidator, IOptions<FssDataMappingConfiguration> fssDataMappingConfiguration)
        {
            _fssEventDataValidator = fssEventDataValidator;
            _fssDataMappingConfiguration = fssDataMappingConfiguration;
        }

        public Task<ValidationResult> ValidateFssEventData(FssEventData fssEventData)
        {
            return _fssEventDataValidator.Validate(fssEventData);
        }

        public CloudEvent FssEventDataMapping(CustomCloudEvent customCloudEvent, string correlationId)
        {
            JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            };

            string data = JsonSerializer.Serialize(customCloudEvent.Data);
            FssEventData fssEventData = JsonSerializer.Deserialize<FssEventData>(data,options);

            fssEventData.Links.BatchStatus.Href = ReplaceHostValueMethod(fssEventData.Links.BatchStatus.Href);
            fssEventData.Links.BatchDetails.Href = ReplaceHostValueMethod(fssEventData.Links.BatchDetails.Href);
            fssEventData.Files.FirstOrDefault().Links.Get.Href = ReplaceHostValueMethod(fssEventData.Files.FirstOrDefault().Links.Get.Href);

            FssDataMappingConfiguration.SourceConfiguration sourceConfiguration = _fssDataMappingConfiguration.Value.Sources
                .FirstOrDefault(x => x.BusinessUnit.Equals(fssEventData.BusinessUnit, StringComparison.OrdinalIgnoreCase));

            if (sourceConfiguration == null)
            {
                throw new ConfigurationMissingException($"Missing FssDataMappingConfiguration configuration for {fssEventData.BusinessUnit} business unit");
            }

            CloudEvent cloudEvent = new(sourceConfiguration.Source, FssDataMappingValueConstant.Type, fssEventData)
            {
                Time = DateTimeOffset.Parse(DateTime.UtcNow.ToRfc3339String()),
                Id = Guid.NewGuid().ToString(),
                Subject = customCloudEvent.Subject,
                DataContentType = customCloudEvent.DataContentType,
                DataSchema = customCloudEvent.DataSchema
            };

            return cloudEvent;
        }

        private string ReplaceHostValueMethod(string href)
        {
            return href.Replace(_fssDataMappingConfiguration.Value.EventHostName, _fssDataMappingConfiguration.Value.PublishHostName);
        }
    }
}
