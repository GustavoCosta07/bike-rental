using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using BikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Infrastructure.Repositories;

public class MotorcycleRepository : IMotorcycleRepository
{
    private readonly BikeRentalDbContext _context;

    public MotorcycleRepository(BikeRentalDbContext context)
    {
        _context = context;
    }

    public async Task<Motorcycle> AddAsync(Motorcycle motorcycle)
    {
        await _context.Motorcycles.AddAsync(motorcycle);
        await _context.SaveChangesAsync();
        return motorcycle;
    }

    public async Task UpdateAsync(Motorcycle motorcycle)
    {
        _context.Motorcycles.Update(motorcycle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Motorcycle motorcycle)
    {
        _context.Motorcycles.Remove(motorcycle);
        await _context.SaveChangesAsync();
    }

    public async Task<Motorcycle?> GetByIdAsync(Guid id)
    {
        return await _context.Motorcycles.FindAsync(id);
    }

    public async Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlate = null)
    {
        var query = _context.Motorcycles.AsQueryable();

        if (!string.IsNullOrEmpty(licensePlate))
        {
            query = query.Where(m => m.LicensePlate.Contains(licensePlate));
        }

        return await query.ToListAsync();
    }

    public async Task<bool> LicensePlateExistsAsync(string licensePlate)
    {
        return await _context.Motorcycles
            .AnyAsync(m => m.LicensePlate == licensePlate);
    }
}