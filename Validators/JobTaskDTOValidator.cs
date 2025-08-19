using FluentValidation;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Validators
{
    public class JobTaskDTOValidator : AbstractValidator<JobTaskDTO>
    {
        public JobTaskDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Task name is required.").Length(3, 100).WithMessage("Task name must be between 3 and 100 characters.");
            RuleFor(x => x.Description).Length(0, 500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority.");
            RuleFor(x => x.JobId).NotEmpty().WithMessage("Job ID is required.");
        }
    }
}
