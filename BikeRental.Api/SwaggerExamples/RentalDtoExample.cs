using BikeRental.Application.Models;
using Swashbuckle.AspNetCore.Filters;

namespace BikeRental.Api.SwaggerExamples;

public class DriverLicenseImageDtoExample : IExamplesProvider<DriverLicenseImageDto>
{
    public DriverLicenseImageDto GetExamples()
    {
        return new DriverLicenseImageDto(
            DriverLicenseImageBase64: "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAFhQGAwJ2GfgAAAABJRU5ErkJggg=="
        );
    }
}