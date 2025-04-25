using BikeRental.Application.Models;
using Swashbuckle.AspNetCore.Filters;

namespace BikeRental.Api.SwaggerExamples;

public class MotorcycleCreateDtoExample : IExamplesProvider<MotorcycleCreateDto>
{
    public MotorcycleCreateDto GetExamples()
    {
        return new MotorcycleCreateDto(
            Year: 2023,
            Model: "Honda CB 500",
            LicensePlate: "ABC1D23");
    }
}