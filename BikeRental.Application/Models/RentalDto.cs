using System.Text.Json.Serialization;
using BikeRental.Domain;

namespace BikeRental.Application.Models;

public record RentalDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("daily_rate")] decimal DailyRate,
    [property: JsonPropertyName("delivery_person_id")] string DeliveryPersonId,
    [property: JsonPropertyName("motorcycle_id")] string MotorcycleId,
    [property: JsonPropertyName("start_date")] DateTime StartDate,
    [property: JsonPropertyName("end_date")] DateTime EndDate,
    [property: JsonPropertyName("expected_end_date")] DateTime ExpectedEndDate,
    [property: JsonPropertyName("return_date")] DateTime? ReturnDate,
    [property: JsonPropertyName("plan")] RentalPlan Plan);

public record RentalReturnDto(
    [property: JsonPropertyName("return_date")] DateTime ReturnDate);

public record RentalReturnResponseDto(
    [property: JsonPropertyName("message")] string Message);