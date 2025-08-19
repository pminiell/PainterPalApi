using FluentValidation;
using PainterPalApi.DTOs;

namespace PainterPalApi.Validators
{
    public class QuoteDTOValidator : AbstractValidator<QuoteDTO>
    {
        public QuoteDTOValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
            RuleFor(x => x.QuoteNotes).NotEmpty().WithMessage("Quote notes are required.").Length(10, 500).WithMessage("Quote notes must be between 10 and 500 characters.");
            RuleFor(x => x.QuotePrice).NotEmpty().WithMessage("Quote price is required.").Matches(@"^[0-9]+(\.[0-9]{1,2})?$").WithMessage("Invalid price format.");
            RuleFor(x => x.QuoteStatus).IsInEnum().WithMessage("Invalid quote status.");
        }
    }
}
