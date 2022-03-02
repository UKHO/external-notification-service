using Azure.Messaging;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
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

        public CloudEvent FssEventDataMapping(CustomEventGridEvent customEventGridEvent, string correlationId)
        {
            string data = JsonConvert.SerializeObject(customEventGridEvent.Data);
            FssEventData fssEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            fssEventData.Links.BatchStatus.Href = fssEventData.Links.BatchStatus.Href.Replace(_fssDataMappingConfiguration.Value.ExistingHostName, _fssDataMappingConfiguration.Value.ReplacingHostName);
            fssEventData.Links.BatchDetails.Href = fssEventData.Links.BatchDetails.Href.Replace(_fssDataMappingConfiguration.Value.ExistingHostName, _fssDataMappingConfiguration.Value.ReplacingHostName);
            var linksHref = fssEventData.Files.FirstOrDefault().Links.Get.Href.Replace(_fssDataMappingConfiguration.Value.ExistingHostName, _fssDataMappingConfiguration.Value.ReplacingHostName);
            fssEventData.Files.FirstOrDefault().Links.Get.Href = linksHref; 

            CloudEvent cloudEvent = new(_fssDataMappingConfiguration.Value.Source,
                                        _fssDataMappingConfiguration.Value.Type, 
                                        fssEventData)
            { 
                Time = DateTimeOffset.Parse(DateTimeExtensions.ToRfc3339String(DateTime.UtcNow)),
                Id = Guid.NewGuid().ToString(),
                Subject = customEventGridEvent.Subject,
                DataContentType = customEventGridEvent.DataContentType,
                DataSchema= customEventGridEvent.DataSchema
            };

            return cloudEvent;
        }
    }
}
