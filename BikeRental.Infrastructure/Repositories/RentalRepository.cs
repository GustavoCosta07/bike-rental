namespace BikeRental.Infrastructure.Repositories;

using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using BikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class RentalRepository : IRentalRepository
{
    private readonly BikeRentalDbContext _context;

    public RentalRepository(BikeRentalDbContext context)
    {
        _context = context;
    }

    public async Task<Rental> AddAsync(Rental rental)
    {
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();
        return rental;
    }

    public async Task UpdateAsync(Rental rental)
    {
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await _context.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId)
    {
        return await _context.Rentals
            .AnyAsync(r => r.MotorcycleId == motorcycleId && r.IsActive);
    }
}