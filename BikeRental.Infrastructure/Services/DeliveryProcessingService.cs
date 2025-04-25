using BikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BikeRental.Infrastructure.Services;

public class DeliveryProcessingService : BackgroundService
{
    private readonly ILogger<DeliveryProcessingService> _logger;
    private readonly IServiceProvider _services;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public DeliveryProcessingService(
        ILogger<DeliveryProcessingService> logger,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Delivery Processing Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BikeRentalDbContext>();

                await ProcessPendingDeliveriesAsync(dbContext, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deliveries");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Delivery Processing Service is stopping.");
    }

    private async Task ProcessPendingDeliveriesAsync(BikeRentalDbContext dbContext, CancellationToken cancellationToken)
    {
        var pendingDeliveries = await dbContext.OrderDeliveries
            .Where(od => !od.IsCompleted)
            .ToListAsync(cancellationToken);

        foreach (var delivery in pendingDeliveries)
        {
        }
    }
}