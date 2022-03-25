using Azure.Messaging;
using FluentValidation.Results;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IScsEventValidationAndMappingService
    {
        Task<ValidationResult> ValidateScsEventData(ScsEventData scsEventData);

        CloudEvent ScsEventDataMapping(CustomCloudEvent customCloudEvent, string correlationId);
    }
}
