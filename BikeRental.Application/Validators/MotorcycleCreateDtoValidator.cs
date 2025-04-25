using BikeRental.Application.Models;
using FluentValidation;

namespace BikeRental.Application.Validators;

public class MotorcycleCreateDtoValidator : AbstractValidator<MotorcycleCreateDto>
{
    public MotorcycleCreateDtoValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.Now.Year + 1);

        RuleFor(x => x.Model)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .Matches(@"^[A-Z]{3}\d{1}[A-Z]{1}\d{2}$")
            .WithMessage("License plate must be in the format AAA1A11");
    }
}