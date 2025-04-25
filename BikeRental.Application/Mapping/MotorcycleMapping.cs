using BikeRental.Application.Models;
using BikeRental.Domain.Entities;

namespace BikeRental.Application.Mapping
{
    public static class MotorcycleMapping
    {
        public static MotorcycleDto MapToDto(this Motorcycle motorcycle)
        {
           return new MotorcycleDto(motorcycle.Id, motorcycle.Year, motorcycle.Model, motorcycle.LicensePlate, motorcycle.CreatedAt);
        }
    }
}
