using FluentValidation;
using PainterPalApi.DTOs;

namespace PainterPalApi.Validators
{
    public class MaterialDTOValidator : AbstractValidator<MaterialDTO>
    {
        public MaterialDTOValidator()
        {
            RuleFor(x => x.MaterialName).NotEmpty().WithMessage("Material name is required.").Length(3, 100).WithMessage("Material name must be between 3 and 100 characters.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required.").Length(3, 50).WithMessage("Category must be between 3 and 50 characters.");
            RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than 0.");
            RuleFor(x => x.Supplier).NotEmpty().WithMessage("Supplier is required.").Length(3, 100).WithMessage("Supplier must be between 3 and 100 characters.");
        }
    }
}
