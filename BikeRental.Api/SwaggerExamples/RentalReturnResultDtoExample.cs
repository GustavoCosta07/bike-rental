using BikeRental.Application.Models;
using BikeRental.Domain;
using Swashbuckle.AspNetCore.Filters;

public class RentalReturnResultDtoExample : IExamplesProvider<RentalReturnResultDto>
{
    public RentalReturnResultDto GetExamples()
    {
        return new RentalReturnResultDto(
            Rental: new RentalDto(
                Id: "789a123b-456c-789d-123e-456f789a123b",
                DailyRate: 30.00m,
                DeliveryPersonId: "123e4567-e89b-12d3-a456-426614174000",
                MotorcycleId: "a1b2c3d4-1234-5678-9012-abcdef123456",
                StartDate: new DateTime(2024, 1, 1),
                EndDate: new DateTime(2024, 1, 8),
                ExpectedEndDate: new DateTime(2024, 1, 8),
                ReturnDate: new DateTime(2024, 1, 5),
                Plan: RentalPlan.SevenDays
            ),
            TotalCost: 150.00m,
            Penalty: 18.00m
        );
    }
}