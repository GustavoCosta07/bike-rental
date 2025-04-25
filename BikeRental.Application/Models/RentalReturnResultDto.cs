namespace BikeRental.Application.Models
{
    public record RentalReturnResultDto(
        RentalDto Rental,
        decimal TotalCost,
        decimal Penalty
    );
}