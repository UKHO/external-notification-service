using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface ID365PayloadValidator
    {
        Task<ValidationResult> Validate(D365Payload d365Payload);
    }
    public class D365PayloadValidator : AbstractValidator<D365Payload>, ID365PayloadValidator
    {
        public D365PayloadValidator()
        {
            RuleFor(v => v.CorrelationId).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload CorrelationId cannot be blank or null.");

            RuleFor(v => v.InputParameters).NotNull()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload InputParameters cannot be blank or null.");

            RuleFor(v => v.PostEntityImages).NotNull()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365Payload PostEntityImages cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("SubscriptionId")
                .Must(x => x.IsValidSubscriptionId())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("SubscriptionId cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("NotificationType")
                .Must(x => x.IsValidNotificationType())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("NotificationType cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("WebhookUrl")
                .Must(x => x.IsValidWebhookUrl())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("WebhookUrl cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("StateCode")
                .Must(x => x.IsValidStatus())
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("StateCode cannot be blank or null.");
        }
        Task<ValidationResult> ID365PayloadValidator.Validate(D365Payload d365Payload)
        {
            return ValidateAsync(d365Payload);
        }
    }
}
