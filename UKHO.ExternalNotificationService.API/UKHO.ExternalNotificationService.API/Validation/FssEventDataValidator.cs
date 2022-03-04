using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.Validation
{
    public interface IFssEventDataValidator
    { 
        Task<ValidationResult> Validate(FssEventData fssEventData);
    }

    public class FssEventDataValidator : AbstractValidator<FssEventData>, IFssEventDataValidator
    {
        public FssEventDataValidator()
        {
            RuleFor(v => v.BatchId).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("BatchId cannot be blank or null.");

            RuleFor(v => v.BusinessUnit).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Business unit cannot be blank or null.");

            RuleFor(v => v.BatchPublishedDate).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Batch published date cannot be blank or null.");

            RuleFor(v => v.Links.BatchDetails).NotNull().NotEmpty().OverridePropertyName("BatchDetails")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Batch details links cannot be blank or null.");

            RuleFor(v => v.Links.BatchStatus).NotNull().NotEmpty().OverridePropertyName("BatchStatus")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Batch status links cannot be blank or null.");

            RuleFor(v => v.Files).NotNull().NotEmpty().OverridePropertyName("Links")
                .Must(at => at.All(a => a.Links != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File links cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("MIMEType")
                .Must(at => at.All(a => a.MIMEType != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File MIME type cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("Hash")
                .Must(at => at.All(a => a.Hash != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File hash cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("FileName")
                .Must(at => at.All(a => a.FileName != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File name cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("FileSize")
                .Must(at => at.All(a => a.FileSize > 0))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File size cannot be null.");
        }

        Task<ValidationResult> IFssEventDataValidator.Validate(FssEventData fssEventData)
        {
            return ValidateAsync(fssEventData);
        }
    }
}
