namespace BikeRental.Domain.Events;

public record MotorcycleRegisteredEvent(
    Guid MotorcycleId,
    int Year,
    string LicensePlate);