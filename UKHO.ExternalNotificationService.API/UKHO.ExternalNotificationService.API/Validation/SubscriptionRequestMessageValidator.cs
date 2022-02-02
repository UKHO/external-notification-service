using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface ISubscriptionRequestMessageValidator
    {
        Task<ValidationResult> Validate(SubscriptionRequest subscriptionRequest);
    }
    public class SubscriptionRequestMessageValidator : AbstractValidator<SubscriptionRequest>, ISubscriptionRequestMessageValidator
    {
        public SubscriptionRequestMessageValidator()
        {
            RuleFor(p => p.SubscriptionId).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("subscriptionId cannot be blank or null.");

            RuleFor(p => p.NotificationType).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("notificationType cannot be blank or null.");

            RuleFor(p => p.IsActive).NotEmpty().NotNull()
                .Must(x => x == false || x == true)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("isActive cannot be blank or null.");

            RuleFor(p => p.WebhookUrl).NotEmpty().NotNull()
                .Must(ru => !string.IsNullOrWhiteSpace(ru))
                .When(ru => ru != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("webhookUrl cannot be blank or null.");
        }
        Task<ValidationResult> ISubscriptionRequestMessageValidator.Validate(SubscriptionRequest subscriptionRequest)
        {
            return ValidateAsync(subscriptionRequest);
        }
    }
}
