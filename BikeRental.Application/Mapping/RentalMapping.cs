using BikeRental.Application.Models;
using BikeRental.Domain.Entities;

public static class RentalMapping
{
    public static RentalDto MapToDto(this Rental rental)
    {
        return new RentalDto(
            Id: rental.Id.ToString(),
            DailyRate: rental.DailyCost,
            DeliveryPersonId: rental.DeliveryPersonId.ToString(),
            MotorcycleId: rental.MotorcycleId.ToString(),
            StartDate: rental.StartDate,
            EndDate: rental.EndDate,
            ExpectedEndDate: rental.ExpectedEndDate,
            ReturnDate: rental.EndDate <= DateTime.UtcNow && !rental.IsActive ? rental.EndDate : null,
            Plan: rental.Plan);
    }
}