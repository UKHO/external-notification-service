using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface ISubscriptionRequestValidator
    {
        Task<ValidationResult> Validate(SubscriptionRequest subscriptionRequest);
    }
    public class SubscriptionRequestValidator : AbstractValidator<SubscriptionRequest>, ISubscriptionRequestValidator
    {
        public SubscriptionRequestValidator()
        {
            RuleFor(p => p.D365CorrelationId).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("D365CorrelationId cannot be blank or null.");

            RuleFor(p => p.SubscriptionId).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("SubscriptionId cannot be blank or null.");

            RuleFor(p => p.NotificationType).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("NotificationType cannot be blank or null.");

            RuleFor(p => p.IsActive).NotEmpty().NotNull()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("IsActive cannot be blank or null.");

            RuleFor(p => p.WebhookUrl).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("WebhookUrl cannot be blank or null.");
        }
        Task<ValidationResult> ISubscriptionRequestValidator.Validate(SubscriptionRequest subscriptionRequest)
        {
            return ValidateAsync(subscriptionRequest);
        }
    }
}
