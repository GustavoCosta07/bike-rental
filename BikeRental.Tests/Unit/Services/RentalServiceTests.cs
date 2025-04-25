using BikeRental.Application.Models;
using BikeRental.Application.Services;
using BikeRental.Domain;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace BikeRental.Tests.Unit.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IValidator<RentalDto>> _createValidatorMock;
    private readonly Mock<IValidator<RentalReturnDto>> _returnValidatorMock;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _createValidatorMock = new Mock<IValidator<RentalDto>>();
        _returnValidatorMock = new Mock<IValidator<RentalReturnDto>>();
        _service = new RentalService(
            _rentalRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _motorcycleRepositoryMock.Object,
            _createValidatorMock.Object,
            _returnValidatorMock.Object);
    }

    [Fact]
    public async Task CreateRentalAsync_ValidDto_CreatesRental()
    {
        var createDto = new RentalDto(
            Id: Guid.NewGuid().ToString(),
            DailyRate: 30.00m,
            DeliveryPersonId: Guid.NewGuid().ToString(),
            MotorcycleId: Guid.NewGuid().ToString(),
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(8),
            ExpectedEndDate: DateTime.UtcNow.AddDays(8),
            ReturnDate: null,
            Plan: RentalPlan.SevenDays
        );
        var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(createDto.DeliveryPersonId), DriverLicenseType = LicenseType.A };
        var motorcycle = new Motorcycle { Id = Guid.Parse(createDto.MotorcycleId) };
        var rental = new Rental
        {
            Id = Guid.Parse(createDto.Id),
            MotorcycleId = Guid.Parse(createDto.MotorcycleId),
            DeliveryPersonId = Guid.Parse(createDto.DeliveryPersonId),
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            ExpectedEndDate = createDto.ExpectedEndDate,
            DailyCost = createDto.DailyRate,
            Plan = createDto.Plan,
            IsActive = true
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _deliveryPersonRepositoryMock.Setup(r => r.GetByIdAsync(Guid.Parse(createDto.DeliveryPersonId)))
            .ReturnsAsync(deliveryPerson);
        _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(Guid.Parse(createDto.MotorcycleId)))
            .ReturnsAsync(motorcycle);
        _rentalRepositoryMock.Setup(r => r.HasActiveRentalForMotorcycleAsync(Guid.Parse(createDto.MotorcycleId)))
            .ReturnsAsync(false);
        _rentalRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Rental>()))
            .ReturnsAsync(rental);

        var result = await _service.CreateRentalAsync(createDto);

        result.Should().NotBeNull();
        result.DailyRate.Should().Be(createDto.DailyRate);
        result.Plan.Should().Be(createDto.Plan);
    }

    [Fact]
    public async Task CreateRentalAsync_InvalidDriverLicenseType_ThrowsValidationException()
    {
        var createDto = new RentalDto(
            Id: Guid.NewGuid().ToString(),
            DailyRate: 30.00m,
            DeliveryPersonId: Guid.NewGuid().ToString(),
            MotorcycleId: Guid.NewGuid().ToString(),
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(8),
            ExpectedEndDate: DateTime.UtcNow.AddDays(8),
            ReturnDate: null,
            Plan: RentalPlan.SevenDays
        );
        var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(createDto.DeliveryPersonId), DriverLicenseType = LicenseType.B };

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _deliveryPersonRepositoryMock.Setup(r => r.GetByIdAsync(Guid.Parse(createDto.DeliveryPersonId)))
            .ReturnsAsync(deliveryPerson);

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _service.CreateRentalAsync(createDto));
    }
 
}