using BikeRental.Domain.Entities;

namespace BikeRental.Domain.Repositories
{
    public interface IRentalRepository
    {
        Task<Rental> AddAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task<Rental?> GetByIdAsync(Guid id);
        Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId);
    }
}