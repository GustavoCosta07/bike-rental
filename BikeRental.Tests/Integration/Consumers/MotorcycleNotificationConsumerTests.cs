using BikeRental.Domain.Events;
using BikeRental.Infrastructure.Consumers;
using BikeRental.Infrastructure.Data;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Tests.Integration.Consumers;

public class MotorcycleNotificationConsumerTests
{
    private readonly BikeRentalDbContext _dbContext;
    private readonly InMemoryTestHarness _harness;

    public MotorcycleNotificationConsumerTests()
    {
        var options = new DbContextOptionsBuilder<BikeRentalDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        _dbContext = new BikeRentalDbContext(options);
        _harness = new InMemoryTestHarness();
        _harness.Consumer(() => new MotorcycleNotificationConsumer(_dbContext));
    }

    [Fact]
    public async Task Consume_MotorcycleRegisteredEvent_Year2024_CreatesNotification()
    {
        var @event = new MotorcycleRegisteredEvent(Guid.NewGuid(), 2024, "ABC1234");

        await _harness.Start();
        await _harness.Bus.Publish(@event);
        await _harness.Consumed.Any<MotorcycleRegisteredEvent>();

        var notifications = await _dbContext.Notifications.ToListAsync();
        notifications.Should().HaveCount(1);
        notifications.First().Message.Should().Be($"New 2024 motorcycle registered: {@event.LicensePlate}");
        notifications.First().IsRead.Should().BeFalse();

        await _harness.Stop();
    }

    [Fact]
    public async Task Consume_MotorcycleRegisteredEvent_Year2023_DoesNotCreateNotification()
    {
        var @event = new MotorcycleRegisteredEvent(Guid.NewGuid(), 2023, "XYZ5678");

        await _harness.Start();
        await _harness.Bus.Publish(@event);
        await _harness.Consumed.Any<MotorcycleRegisteredEvent>();

        var notifications = await _dbContext.Notifications.ToListAsync();
        notifications.Should().BeEmpty();

        await _harness.Stop();
    }
}