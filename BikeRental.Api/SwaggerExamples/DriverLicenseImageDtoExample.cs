using BikeRental.Application.Models;
using Swashbuckle.AspNetCore.Filters;

namespace BikeRental.Api.SwaggerExamples;

public class RentalDtoExample : IExamplesProvider<RentalDto>
{
    public RentalDto GetExamples()
    {
        return new RentalDto(
            Id: Guid.NewGuid().ToString(),
            DailyRate: 30.00m,
            DeliveryPersonId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000").ToString(),
            MotorcycleId: Guid.Parse("a1b2c3d4-1234-5678-9012-abcdef123456").ToString(),
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(8),
            ExpectedEndDate: DateTime.UtcNow.AddDays(8),
            ReturnDate: null,
            Plan: BikeRental.Domain.RentalPlan.SevenDays
        );
    }
}