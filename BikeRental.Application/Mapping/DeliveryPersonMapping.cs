using BikeRental.Application.Models;
using BikeRental.Domain.Entities;

namespace BikeRental.Application.Mapping;

public static class DeliveryPersonMapping
{
    public static DeliveryPersonDto MapToDto(this DeliveryPerson deliveryPerson)
    {
        return new DeliveryPersonDto(
            deliveryPerson.Id.ToString(),
            deliveryPerson.Name,
            deliveryPerson.CNPJ,
            deliveryPerson.BirthDate,
            deliveryPerson.DriverLicenseNumber,
            deliveryPerson.DriverLicenseType.ToString(),
            deliveryPerson.DriverLicenseImageUrl);
    }
}