using BikeRental.Domain.Entities;
using BikeRental.Infrastructure.Data;
using BikeRental.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Tests.Integration.Repositories;

public class MotorcycleRepositoryTests
{
    private readonly BikeRentalDbContext _dbContext;
    private readonly MotorcycleRepository _repository;

    public MotorcycleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BikeRentalDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        _dbContext = new BikeRentalDbContext(options);
        _repository = new MotorcycleRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ValidMotorcycle_SavesToDatabase()
    {
        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            Year = 2023,
            Model = "Honda CB 500",
            LicensePlate = "ABC1D23"
        };

        var result = await _repository.AddAsync(motorcycle);
        await _dbContext.SaveChangesAsync();

        var savedMotorcycle = await _dbContext.Motorcycles.FindAsync(motorcycle.Id);
        savedMotorcycle.Should().NotBeNull();
        savedMotorcycle!.Year.Should().Be(motorcycle.Year);
        savedMotorcycle.LicensePlate.Should().Be(motorcycle.LicensePlate);
    }

    [Fact]
    public async Task LicensePlateExistsAsync_ExistingPlate_ReturnsTrue()
    {
        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            Year = 2023,
            Model = "Honda CB 500",
            LicensePlate = "ABC1D23"
        };
        await _dbContext.Motorcycles.AddAsync(motorcycle);
        await _dbContext.SaveChangesAsync();

        var exists = await _repository.LicensePlateExistsAsync("ABC1D23");

        exists.Should().BeTrue();
    }
}