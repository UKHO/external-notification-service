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
            RuleFor(p => p.InputParameters).NotEmpty().NotNull()
              .WithErrorCode(HttpStatusCode.OK.ToString());
        }
        Task<ValidationResult> ID365PayloadValidator.Validate(D365Payload d365Payload)
        {
            return ValidateAsync(d365Payload);
        }
    }
}
