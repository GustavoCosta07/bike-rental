namespace BikeRental.Api;

public static class CachingExtensions
{
    public static IServiceCollection AddCachingConfiguration(this IServiceCollection services)
    {
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024;
            options.UseCaseSensitivePaths = true;
        });

        return services;
    }
}