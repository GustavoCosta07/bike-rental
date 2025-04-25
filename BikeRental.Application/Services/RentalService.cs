using BikeRental.Application.Models;
using BikeRental.Domain;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using FluentValidation;

namespace BikeRental.Application.Services;

public class RentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IValidator<RentalDto> _createValidator;
    private readonly IValidator<RentalReturnDto> _returnValidator;

    public RentalService(
        IRentalRepository rentalRepository,
        IDeliveryPersonRepository deliveryPersonRepository,
        IMotorcycleRepository motorcycleRepository,
        IValidator<RentalDto> createValidator,
        IValidator<RentalReturnDto> returnValidator)
    {
        _rentalRepository = rentalRepository;
        _deliveryPersonRepository = deliveryPersonRepository;
        _motorcycleRepository = motorcycleRepository;
        _createValidator = createValidator;
        _returnValidator = returnValidator;
    }

    public async Task<RentalDto> CreateRentalAsync(RentalDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(Guid.Parse(createDto.DeliveryPersonId));
        if (deliveryPerson == null)
        {
            throw new KeyNotFoundException("Delivery person not found");
        }

        if (deliveryPerson.DriverLicenseType != LicenseType.A && deliveryPerson.DriverLicenseType != LicenseType.AB)
        {
            throw new FluentValidation.ValidationException("Delivery person must have a type A or AB driver's license");
        }

        var motorcycle = await _motorcycleRepository.GetByIdAsync(Guid.Parse(createDto.MotorcycleId));
        if (motorcycle == null)
        {
            throw new KeyNotFoundException("Motorcycle not found");
        }

        if (await _rentalRepository.HasActiveRentalForMotorcycleAsync(Guid.Parse(createDto.MotorcycleId)))
        {
            throw new InvalidOperationException("Motorcycle is already rented");
        }

        var dailyCosts = new Dictionary<RentalPlan, decimal>
        {
            { RentalPlan.SevenDays, 30.00m },
            { RentalPlan.FifteenDays, 28.00m },
            { RentalPlan.ThirtyDays, 22.00m },
            { RentalPlan.FortyFiveDays, 20.00m },
            { RentalPlan.FiftyDays, 18.00m }
        };

        var dailyCost = dailyCosts[createDto.Plan];
        var creationDate = DateTime.UtcNow;
        var expectedStartDate = creationDate.Date.AddDays(1);

        if (createDto.StartDate.Date != expectedStartDate)
        {
            throw new FluentValidation.ValidationException("The start date must be the day after the creation date");
        }

        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            MotorcycleId = Guid.Parse(createDto.MotorcycleId),
            DeliveryPersonId = Guid.Parse(createDto.DeliveryPersonId),
            StartDate = DateTime.SpecifyKind(createDto.StartDate, DateTimeKind.Unspecified),
            EndDate = DateTime.SpecifyKind(createDto.EndDate, DateTimeKind.Unspecified),
            ExpectedEndDate = DateTime.SpecifyKind(createDto.ExpectedEndDate, DateTimeKind.Unspecified),
            DailyCost = dailyCost,
            Plan = createDto.Plan,
            IsActive = true,
            ReturnDate = null
        };

        await _rentalRepository.AddAsync(rental);

        return rental.MapToDto();
    }

    public async Task<RentalDto?> GetRentalByIdAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        return rental?.MapToDto();
    }

    public async Task<RentalDto> ReturnRentalAsync(Guid id, RentalReturnDto returnDto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        if (!rental.IsActive)
        {
            throw new InvalidOperationException("Rental is not active");
        }

        var returnDate = DateTime.SpecifyKind(returnDto.ReturnDate, DateTimeKind.Unspecified);
        rental.ReturnDate = returnDate;
        rental.IsActive = false;

        var totalCost = CalculateTotalCost(rental, returnDate);
        rental.DailyCost = totalCost / (rental.ExpectedEndDate - rental.StartDate).Days;

        await _rentalRepository.UpdateAsync(rental);

        return rental.MapToDto();
    }

    private decimal CalculateTotalCost(Rental rental, DateTime returnDate)
    {
        var daysUsed = (returnDate - rental.StartDate).Days;
        if (daysUsed < 1) daysUsed = 1;

        var baseCost = daysUsed * rental.DailyCost;
        decimal penalty = 0;

        var expectedDays = (rental.ExpectedEndDate - rental.StartDate).Days;
        if (returnDate < rental.ExpectedEndDate)
        {
            var unusedDays = expectedDays - daysUsed;
            penalty = unusedDays * rental.DailyCost * 0.2m;
        }
        else if (returnDate > rental.ExpectedEndDate)
        {
            var extraDays = daysUsed - expectedDays;
            penalty = extraDays * rental.DailyCost * 0.4m;
        }

        return baseCost + penalty;
    }
}