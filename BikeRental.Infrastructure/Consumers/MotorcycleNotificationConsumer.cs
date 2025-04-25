using BikeRental.Domain.Events;
using BikeRental.Domain.Entities;
using BikeRental.Infrastructure.Data;
using MassTransit;

namespace BikeRental.Infrastructure.Consumers;

public class MotorcycleNotificationConsumer : IConsumer<MotorcycleRegisteredEvent>
{
    private readonly BikeRentalDbContext _dbContext;

    public MotorcycleNotificationConsumer(BikeRentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<MotorcycleRegisteredEvent> context)
    {
        var message = context.Message;

        if (message.Year == 2024)
        {
            var notification = new Notification
            {
                Message = $"New 2024 motorcycle registered: {message.LicensePlate}",
                NotificationDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                IsRead = false
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}