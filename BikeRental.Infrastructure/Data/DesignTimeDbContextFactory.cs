using BikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BikeRental.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BikeRentalDbContext>
    {
        public BikeRentalDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<BikeRentalDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new BikeRentalDbContext(optionsBuilder.Options);
        }
    }
}
