using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.API.Extensions;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface ID365PayloadValidator
    {
        Task<ValidationResult> Validate(D365PayloadValidation d365PayloadValidation);
    }
    public class D365PayloadValidator : AbstractValidator<D365PayloadValidation>, ID365PayloadValidator
    {
        public D365PayloadValidator()
        {
            RuleFor(v => v.D365Payload.CorrelationId).NotNull().NotEmpty().OverridePropertyName("CorrelationId")
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload CorrelationId cannot be blank or null.");

            RuleFor(v => v.D365Payload.InputParameters).NotNull()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload InputParameters cannot be blank or null.");

            RuleFor(v => v.D365Payload.PostEntityImages).NotNull().OverridePropertyName("PostEntityImages")
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload PostEntityImages cannot be blank or null.");

            RuleFor(v => v.D365Payload).NotNull().OverridePropertyName("SubscriptionId")
                .Must(x => x.IsValidSubscriptionId())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("SubscriptionId cannot be blank or null.");

            RuleFor(v => v.D365Payload).NotNull().OverridePropertyName("NotificationType")
                .Must(x => x.IsValidNotificationType())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("NotificationType cannot be blank or null.");

            RuleFor(v => v.D365Payload).NotNull().OverridePropertyName("WebhookUrl")
                .Must(x => x.IsValidWebhookUrl())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("WebhookUrl cannot be blank or null.");
        }
        Task<ValidationResult> ID365PayloadValidator.Validate(D365PayloadValidation d365PayloadValidation)
        {
            return ValidateAsync(d365PayloadValidation);
        }
    }
}
