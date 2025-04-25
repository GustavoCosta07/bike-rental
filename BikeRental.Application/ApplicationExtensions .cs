using System.Reflection;
using BikeRental.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BikeRental.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MotorcycleService>();
        services.AddScoped<DeliveryPersonService>();
        services.AddScoped<RentalService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}