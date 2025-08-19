using FluentValidation;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Validators
{
    public class JobDTOValidator : AbstractValidator<JobDTO>
    {
        public JobDTOValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
            RuleFor(x => x.JobName).NotEmpty().WithMessage("Job name is required.").Length(3, 100).WithMessage("Job name must be between 3 and 100 characters.");
            RuleFor(x => x.JobLocation).NotEmpty().WithMessage("Job location is required.").Length(3, 200).WithMessage("Job location must be between 3 and 200 characters.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start date is required.");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("End date is required.").GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date.");
            RuleFor(x => x.JobStatus).IsInEnum().WithMessage("Invalid job status.");
        }
    }
}
