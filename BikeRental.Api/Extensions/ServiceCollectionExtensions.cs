using BikeRental.Infrastructure.Data;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => false;

            options.Map<InvalidOperationException>(ex => new ProblemDetails
            {
                Title = "Invalid operation",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });

        });

        services.AddHealthChecks()
            .AddDbContextCheck<BikeRentalDbContext>();

        return services;
    }
}