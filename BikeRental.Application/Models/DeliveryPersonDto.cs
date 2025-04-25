using System.Text.Json.Serialization;

namespace BikeRental.Application.Models;

public record DeliveryPersonCreateDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("tax_id")] string TaxId,
    [property: JsonPropertyName("birth_date")] DateTime BirthDate,
    [property: JsonPropertyName("driver_license_number")] string DriverLicenseNumber,
    [property: JsonPropertyName("driver_license_type")] string DriverLicenseType,
    [property: JsonPropertyName("driver_license_image")] string DriverLicenseImageBase64);

public record DeliveryPersonDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("tax_id")] string TaxId,
    [property: JsonPropertyName("birth_date")] DateTime BirthDate,
    [property: JsonPropertyName("driver_license_number")] string DriverLicenseNumber,
    [property: JsonPropertyName("driver_license_type")] string DriverLicenseType,
    [property: JsonPropertyName("driver_license_image")] string DriverLicenseImageUrl);

public record DriverLicenseImageDto(
    [property: JsonPropertyName("driver_license_image")] string DriverLicenseImageBase64);