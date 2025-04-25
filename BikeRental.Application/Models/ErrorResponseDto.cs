using System.Text.Json.Serialization;

namespace BikeRental.Application.Models;

public record ErrorResponseDto(
    [property: JsonPropertyName("message")] string Message);