using System.Threading.Tasks;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface ISubscriptionService
    {
        Task<ValidationResult> ValidateD365PayloadRequest(D365PayloadValidation d365PayloadValidation);

        SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload payload);
    }
}
