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
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("BatchId cannot be blank or null.");

            RuleFor(v => v.BusinessUnit).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("Business unit cannot be blank or null.");

            RuleFor(v => v.BatchPublishedDate).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("Batch published date cannot be blank or null.");

            RuleFor(b => b.Attributes)
                .Must(pi => pi != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("Attributes filed cannot be blank or null.");

            RuleFor(v => v.Links.BatchDetails).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("Batch details links cannot be blank or null.");

            RuleFor(v => v.Links.BatchStatus).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("Batch status links cannot be blank or null.");

            RuleFor(v => v.Files).NotNull().NotEmpty()
                .Must(at => at.All(a => !string.IsNullOrWhiteSpace(a.Links.ToString())))
                .When(ru => ru.Attributes != null)
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File links are missing in event data.");

            ////------
            
            RuleFor(b => b.Files)
                .Must(at => at.All(a => a.MIMEType != null))
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File MIME type cannot be null.");

            RuleFor(b => b.Files)
                .Must(at => at.All(a => a.Attributes != null))
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File attributes cannot be null.");

            RuleFor(b => b.Files)
                .Must(at => at.All(a => a.Hash != null))
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File hash cannot be null.");

            RuleFor(b => b.Files)
                .Must(at => at.All(a => a.FileName != null))
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File name cannot be null.");

            RuleFor(b => b.Files)
                .Must(at => at.All(a => a.FileSize > 0))
                .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                .WithMessage("File size cannot be null.");

            ////------
        }

        Task<ValidationResult> IFssEventDataValidator.Validate(FssEventData fssEventData)
        {
            return ValidateAsync(fssEventData);
        }
    }
}
