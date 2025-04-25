using Amazon.SimpleNotificationService;
using Amazon.SQS;
using BikeRental.Application.Services;
using BikeRental.Domain.Repositories;
using BikeRental.Infrastructure.Consumers;
using BikeRental.Infrastructure.Data;
using BikeRental.Infrastructure.Repositories;
using BikeRental.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BikeRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BikeRentalDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();

        services.AddScoped<MotorcycleService>();
        services.AddScoped<DeliveryPersonService>();
        services.AddScoped<RentalService>();

        var awsConfig = configuration.GetSection("AWS");
        var region = awsConfig["Region"];
        var accessKey = awsConfig["AccessKey"];
        var secretKey = awsConfig["SecretKey"];
        var serviceUrl = awsConfig["ServiceURL"];

        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<MotorcycleNotificationConsumer>();

            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    h.AccessKey(accessKey);
                    h.SecretKey(secretKey);
                    h.Config(new AmazonSQSConfig
                    {
                        ServiceURL = serviceUrl,
                        UseHttp = true,
                        AuthenticationRegion = region
                    });
                    h.Config(new AmazonSimpleNotificationServiceConfig
                    {
                        ServiceURL = serviceUrl,
                        UseHttp = true,
                        AuthenticationRegion = region
                    });
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}