using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;
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
                .Must(x => x.IsValidAttribute(D365PayloadKeyConstant.PostEntityImageKey, D365PayloadKeyConstant.SubscriptionIdKey))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("SubscriptionId cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("NotificationType")
                .Must(x => x.ContainsFormattedValue(D365PayloadKeyConstant.PostEntityImageKey, D365PayloadKeyConstant.NotificationTypeKey))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("NotificationType cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("WebhookUrl")
                .Must(x => x.IsValidAttribute(D365PayloadKeyConstant.PostEntityImageKey, D365PayloadKeyConstant.WebhookUrlKey))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("WebhookUrl cannot be blank or null.");

            RuleFor(v => v).NotNull().NotEmpty().OverridePropertyName("StateCode")
                .Must(x => x.ContainsFormattedValue(D365PayloadKeyConstant.PostEntityImageKey, D365PayloadKeyConstant.IsActiveKey))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("StateCode cannot be blank or null.");
        }

        [ExcludeFromCodeCoverage]
        Task<ValidationResult> ID365PayloadValidator.Validate(D365Payload d365Payload)
        {
            return ValidateAsync(d365Payload);
        }
    }
}
