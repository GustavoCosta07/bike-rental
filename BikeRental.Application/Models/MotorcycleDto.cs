namespace BikeRental.Application.Models;

public record MotorcycleDto(
    Guid Id,
    int Year,
    string Model,
    string LicensePlate,
    DateTime CreatedAt);

public record MotorcycleCreateDto(
    int Year,
    string Model,
    string LicensePlate);

public record MotorcycleUpdateDto(
    string LicensePlate);