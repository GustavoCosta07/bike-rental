using BikeRental.Domain;
using BikeRental.Domain.Entities;

namespace BikeRental.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

    public class BikeRentalDbContext : DbContext
    {
        public BikeRentalDbContext() { }

        public BikeRentalDbContext(DbContextOptions<BikeRentalDbContext> options) : base(options)
        {
        }

        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<OrderDelivery> OrderDeliveries { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp without time zone");
                    }
                }
            }

            modelBuilder.Entity<DeliveryPerson>()
                .Property(d => d.DriverLicenseType)
                .HasConversion<string>()
                .HasMaxLength(3);

            modelBuilder.Entity<DeliveryPerson>()
                .HasIndex(d => d.CNPJ)
                .IsUnique();

            modelBuilder.Entity<DeliveryPerson>()
                .HasIndex(d => d.DriverLicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Motorcycle>()
                .HasIndex(m => m.LicensePlate)
                .IsUnique();

            modelBuilder.Entity<Motorcycle>().HasData(
                new Motorcycle
                {
                    Id = Guid.Parse("a1b2c3d4-1234-5678-9012-abcdef123456"),
                    Year = 2023,
                    Model = "Honda CB 500",
                    LicensePlate = "ABC1234",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0)
                }
            );

            modelBuilder.Entity<DeliveryPerson>().HasData(
                new DeliveryPerson
                {
                    Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                    Name = "John Doe",
                    CNPJ = "12345678901234",
                    BirthDate = new DateTime(1990, 1, 1, 0, 0, 0),
                    DriverLicenseNumber = "12345678900",
                    DriverLicenseType = LicenseType.A,
                    DriverLicenseImageUrl = "/Storage/john_doe_driver_license.png",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0)
                }
            );

            modelBuilder.Entity<Rental>().HasData(
                new Rental
                {
                    Id = Guid.Parse("789a123b-456c-789d-123e-456f789a123b"),
                    MotorcycleId = Guid.Parse("a1b2c3d4-1234-5678-9012-abcdef123456"),
                    DeliveryPersonId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                    StartDate = new DateTime(2024, 1, 1, 0, 0, 0),
                    EndDate = new DateTime(2024, 1, 8, 0, 0, 0),
                    ExpectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0),
                    DailyCost = 30.00m,
                    Plan = RentalPlan.SevenDays,
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 12, 31, 0, 0, 0)
                }
            );

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Motorcycle)
                .WithMany(m => m.Rentals)
                .HasForeignKey(r => r.MotorcycleId);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.DeliveryPerson)
                .WithMany(d => d.Rentals)
                .HasForeignKey(r => r.DeliveryPersonId);

            modelBuilder.Entity<OrderDelivery>()
                .HasOne(o => o.Rental)
                .WithMany(r => r.OrderDeliveries)
                .HasForeignKey(o => o.RentalId);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                }
                entry.Entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
