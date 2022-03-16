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

            RuleFor(v => v.EditionNumber).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("EditionNumber cannot be less than zero or blank.");

            RuleFor(v => v.UpdateNumber).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("UpdateNumber cannot be less than zero or blank.");

            RuleFor(v => v.FileSize).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("FileSize cannot be less than zero or blank.");

            RuleFor(v => v.BoundingBox).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("BoundingBox cannot be blank or null.");

            RuleFor(v => v.BoundingBox.NorthLimit).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("NorthLimit cannot be less than zero or blank.");

            RuleFor(v => v.BoundingBox.SouthLimit).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("SouthLimit cannot be less than zero or blank.");

            RuleFor(v => v.BoundingBox.EastLimit).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("EastLimit cannot be less than zero or blank.");

            RuleFor(v => v.BoundingBox.WestLimit).NotEmpty().NotNull().GreaterThanOrEqualTo(0)
                .Must(ru => ru >= 0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("WestLimit cannot be less than zero or blank.");

            RuleFor(v => v.Status).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Status cannot be blank or null.");

            RuleFor(v => v.Status.StatusDate).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("StatusDate cannot be blank or null.");

            RuleFor(v => v.Status.StatusName).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("StatusName cannot be blank or null.");
        }

        Task<ValidationResult> IScsEventDataValidator.Validate(ScsEventData scsEventData)
        {
            return ValidateAsync(scsEventData);
        }
    }
}
