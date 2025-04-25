namespace BikeRental.Infrastructure.Repositories;

using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using BikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class DeliveryPersonRepository : IDeliveryPersonRepository
{
    private readonly BikeRentalDbContext _context;

    public DeliveryPersonRepository(BikeRentalDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryPerson> AddAsync(DeliveryPerson deliveryPerson)
    {
        await _context.DeliveryPersons.AddAsync(deliveryPerson);
        await _context.SaveChangesAsync();
        return deliveryPerson;
    }

    public async Task UpdateAsync(DeliveryPerson deliveryPerson)
    {
        _context.DeliveryPersons.Update(deliveryPerson);
        await _context.SaveChangesAsync();
    }

    public async Task<DeliveryPerson?> GetByIdAsync(Guid id)
    {
        return await _context.DeliveryPersons.FindAsync(id);
    }

    public async Task<bool> CnpjExistsAsync(string cnpj)
    {
        return await _context.DeliveryPersons.AnyAsync(d => d.CNPJ == cnpj);
    }

    public async Task<bool> DriverLicenseNumberExistsAsync(string driverLicenseNumber)
    {
        return await _context.DeliveryPersons.AnyAsync(d => d.DriverLicenseNumber == driverLicenseNumber);
    }
}
