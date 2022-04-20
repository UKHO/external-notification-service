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
        private const double LatitudeLimitDegrees = 90;
        private const double LongitudeLimitDegrees = 180;

        public ScsEventDataValidator()
        {
            RuleFor(v => v.ProductType)
                .NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("ProductType cannot be blank or null.");

            RuleFor(v => v.ProductName)
                .NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("ProductName cannot be blank or null.");

            RuleFor(v => v.EditionNumber)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("EditionNumber cannot be less than zero or blank.");

            RuleFor(v => v.UpdateNumber)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("UpdateNumber cannot be less than zero or blank.");

            RuleFor(v => v.FileSize)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("FileSize cannot be less than zero or blank.");

            RuleFor(v => v.BoundingBox)
                .NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("BoundingBox cannot be blank or null.");

            RuleFor(v => v.BoundingBox.NorthLimit)
                .InclusiveBetween(-LatitudeLimitDegrees, LatitudeLimitDegrees)
                .OverridePropertyName("NorthLimit")
                .When(x => x.BoundingBox != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage($"NorthLimit should be in the range -{LatitudeLimitDegrees}.0 to +{LatitudeLimitDegrees}.0.");

            RuleFor(v => v.BoundingBox.SouthLimit)
                .InclusiveBetween(-LatitudeLimitDegrees, LatitudeLimitDegrees)
                .OverridePropertyName("SouthLimit")
                .When(x => x.BoundingBox != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage($"SouthLimit should be in the range -{LatitudeLimitDegrees}.0 to +{LatitudeLimitDegrees}.0.");

            RuleFor(v => v.BoundingBox.EastLimit)
                .InclusiveBetween(-LongitudeLimitDegrees, LongitudeLimitDegrees)
                .OverridePropertyName("EastLimit")
                .When(x => x.BoundingBox != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage($"EastLimit should be in the range -{LongitudeLimitDegrees}.0 to +{LongitudeLimitDegrees}.0.");

            RuleFor(v => v.BoundingBox.WestLimit)
                .InclusiveBetween(-LongitudeLimitDegrees, LongitudeLimitDegrees)
                .OverridePropertyName("WestLimit")
                .When(x => x.BoundingBox != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage($"WestLimit should be in the range -{LongitudeLimitDegrees}.0 to +{LongitudeLimitDegrees}.0.");

            RuleFor(v => v.Status).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Status cannot be blank or null.");

            RuleFor(v => v.Status.StatusDate)
                .NotNull().NotEmpty()
                .OverridePropertyName("StatusDate")
                .When(x => x.Status != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("StatusDate cannot be blank or null.");

            RuleFor(v => v.Status.StatusName)
                .NotNull().NotEmpty()
                .OverridePropertyName("StatusName")
                .When(x => x.Status != null)
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("StatusName cannot be blank or null.");
        }

        Task<ValidationResult> IScsEventDataValidator.Validate(ScsEventData scsEventData)
        {
            return ValidateAsync(scsEventData);
        }
    }
}
