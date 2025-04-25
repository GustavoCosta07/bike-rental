using BikeRental.Application.Models;
using Swashbuckle.AspNetCore.Filters;

namespace BikeRental.Api.SwaggerExamples;

public class DeliveryPersonCreateDtoExample : IExamplesProvider<DeliveryPersonCreateDto>
{
    public DeliveryPersonCreateDto GetExamples()
    {
        return new DeliveryPersonCreateDto(
            Id: Guid.NewGuid().ToString(),
            Name: "João da Silva",
            TaxId: "12345678901231",
            BirthDate: DateTime.ParseExact("1990-08-22", "yyyy-MM-dd", null),
            DriverLicenseNumber: "12345678901",
            DriverLicenseType: "AB",
            DriverLicenseImageBase64: "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAFhQGAwJ2GfgAAAABJRU5ErkJggg=="
        );
    }
}
