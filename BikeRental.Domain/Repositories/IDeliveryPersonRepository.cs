using BikeRental.Domain.Entities;
namespace BikeRental.Domain.Repositories;

public interface IDeliveryPersonRepository
{
    Task<DeliveryPerson> AddAsync(DeliveryPerson deliveryPerson);
    Task UpdateAsync(DeliveryPerson deliveryPerson);
    Task<DeliveryPerson?> GetByIdAsync(Guid id);
    Task<bool> CnpjExistsAsync(string cnpj);
    Task<bool> DriverLicenseNumberExistsAsync(string driverLicenseNumber);
}
