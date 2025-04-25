using BikeRental.Application.Models;
using BikeRental.Application.Services;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Events;
using BikeRental.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using MassTransit;
using Moq;

namespace BikeRental.Tests.Unit.Services;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<IValidator<MotorcycleCreateDto>> _validatorMock;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _validatorMock = new Mock<IValidator<MotorcycleCreateDto>>();
        _service = new MotorcycleService(_motorcycleRepositoryMock.Object, _publishEndpointMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task CreateMotorcycleAsync_ValidDto_CreatesMotorcycleAndPublishesEvent()
    {
        var createDto = new MotorcycleCreateDto(2024, "Honda CB 500", "ABC1234");
        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            Year = createDto.Year,
            Model = createDto.Model,
            LicensePlate = createDto.LicensePlate,
            CreatedAt = DateTime.UtcNow
        };

        _validatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _motorcycleRepositoryMock.Setup(r => r.LicensePlateExistsAsync(createDto.LicensePlate))
            .ReturnsAsync(false);
        _motorcycleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Motorcycle>()))
            .ReturnsAsync(motorcycle);

        var result = await _service.CreateMotorcycleAsync(createDto);

        result.Should().NotBeNull();
        result.Year.Should().Be(createDto.Year);
        result.Model.Should().Be(createDto.Model);
        result.LicensePlate.Should().Be(createDto.LicensePlate);
        _publishEndpointMock.Verify(p => p.Publish(It.Is<MotorcycleRegisteredEvent>(e =>
            e.MotorcycleId == motorcycle.Id &&
            e.Year == motorcycle.Year &&
            e.LicensePlate == motorcycle.LicensePlate), default), Times.Once());
    }

    [Fact]
    public async Task CreateMotorcycleAsync_DuplicateLicensePlate_ThrowsInvalidOperationException()
    {
        var createDto = new MotorcycleCreateDto(2024, "Honda CB 500", "ABC1234");
        _validatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _motorcycleRepositoryMock.Setup(r => r.LicensePlateExistsAsync(createDto.LicensePlate))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateMotorcycleAsync(createDto));
    }

    [Fact]
    public async Task GetAllMotorcyclesAsync_WithLicensePlate_ReturnsFilteredMotorcycles()
    {
        var licensePlate = "ABC";
        var motorcycles = new List<Motorcycle>
        {
            new Motorcycle { Id = Guid.NewGuid(), Year = 2023, Model = "Honda CB 500", LicensePlate = "ABC1234" },
            new Motorcycle { Id = Guid.NewGuid(), Year = 2022, Model = "Yamaha MT-07", LicensePlate = "XYZ5678" }
        };
        _motorcycleRepositoryMock.Setup(r => r.GetAllAsync(licensePlate))
            .ReturnsAsync(motorcycles.Where(m => m.LicensePlate.Contains(licensePlate)));

        var result = await _service.GetAllMotorcyclesAsync(licensePlate);

        result.Should().HaveCount(1);
        result.First().LicensePlate.Should().Be("ABC1234");
    }
}