using FluentValidation;
using FluentValidation.Results;
using System.Net;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface IScsEventDataValidator
    {
        Task<ValidationResult> Validate(ScsEventData scsEventData);
    }

    public class ScsEventDataValidator : AbstractValidator<ScsEventData>, IScsEventDataValidator
    {
        public ScsEventDataValidator()
        {
            RuleFor(v => v.ProductType).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("ProductType cannot be blank or null.");

            RuleFor(v => v.ProductName).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("ProductName cannot be blank or null.");
        }

        Task<ValidationResult> IScsEventDataValidator.Validate(ScsEventData scsEventData)
        {
            return ValidateAsync(scsEventData);
        }
    }
}
