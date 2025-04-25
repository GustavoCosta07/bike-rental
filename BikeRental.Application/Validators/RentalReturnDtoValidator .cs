using BikeRental.Application.Models;
using FluentValidation;

namespace BikeRental.Application.Validators;

public class RentalReturnDtoValidator : AbstractValidator<RentalReturnDto>
{
    public RentalReturnDtoValidator()
    {
        RuleFor(x => x.ReturnDate)
            .NotEmpty()
            .WithMessage("A data de devolução é obrigatória")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("A data de devolução não pode ser no passado");
    }
}