using BikeRental.Application.Models;
using BikeRental.Domain;
using FluentValidation;

namespace BikeRental.Application.Validators;

public class RentalCreateDtoValidator : AbstractValidator<RentalDto>
{
    public RentalCreateDtoValidator()
    {
        RuleFor(x => x.DeliveryPersonId)
            .NotEmpty()
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("Delivery person ID must be a valid GUID");

        RuleFor(x => x.MotorcycleId)
            .NotEmpty()
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("Motorcycle ID must be a valid GUID");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Start date must be at least one day after creation");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date");

        RuleFor(x => x.ExpectedEndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Expected end date must be on or after start date");

        RuleFor(x => x.Plan)
            .Must(plan => Enum.IsDefined(typeof(RentalPlan), plan))
            .WithMessage("Plan must be 7, 15, 30, 45, or 50 days");
    }
}