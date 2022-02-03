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
              .WithMessage("CorrelationId cannot be blank or null.");

            RuleFor(v => v.InputParameters).NotNull()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("InputParameters cannot be null.");

            RuleFor(v => v.PostEntityImages).NotNull()
               .WithErrorCode(HttpStatusCode.BadRequest.ToString())
               .WithMessage("PostEntityImages cannot be null.");
        }
        Task<ValidationResult> ID365PayloadValidator.Validate(D365Payload d365Payload)
        {
            return ValidateAsync(d365Payload);
        }
    }
}
