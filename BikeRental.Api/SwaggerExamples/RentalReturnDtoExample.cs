using BikeRental.Application.Models;
using Swashbuckle.AspNetCore.Filters;

namespace BikeRental.Api.SwaggerExamples;

public class RentalReturnDtoExample : IExamplesProvider<RentalReturnDto>
{
    public RentalReturnDto GetExamples()
    {
        return new RentalReturnDto(
            ReturnDate: DateTime.UtcNow.AddDays(7)
        );
    }
}