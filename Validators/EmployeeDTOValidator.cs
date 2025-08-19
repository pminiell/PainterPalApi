using FluentValidation;
using PainterPalApi.DTOs;

namespace PainterPalApi.Validators
{
    public class EmployeeDTOValidator : AbstractValidator<EmployeeDTO>
    {
        public EmployeeDTOValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.").Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.").IsEnumName(typeof(Models.UserRole), caseSensitive: false).WithMessage("Invalid role.");
        }
    }
}
