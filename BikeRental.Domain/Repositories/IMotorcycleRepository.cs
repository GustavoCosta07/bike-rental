using BikeRental.Domain.Entities;

namespace BikeRental.Domain.Repositories;

public interface IMotorcycleRepository
{
    Task<Motorcycle> AddAsync(Motorcycle motorcycle);
    Task UpdateAsync(Motorcycle motorcycle);
    Task DeleteAsync(Motorcycle motorcycle);
    Task<Motorcycle?> GetByIdAsync(Guid id);
    Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlate = null);
    Task<bool> LicensePlateExistsAsync(string licensePlate);
}