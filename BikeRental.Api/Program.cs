using System.Reflection;
using AspNetCoreRateLimit;
using BikeRental.Api;
using BikeRental.Api.HealthChecks;
using BikeRental.Api.Middlewares;
using BikeRental.Api.SwaggerExamples;
using BikeRental.Application;
using BikeRental.Infrastructure;
using BikeRental.Infrastructure.Data;
using BikeRental.Infrastructure.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureLogging();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices();

builder.Services.AddHealthChecks()
    .AddCheck<FileStorageHealthCheck>("file_storage");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bike Rental API", Version = "v1" });

    c.ExampleFilters();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddSwaggerExamplesFromAssemblyOf<MotorcycleCreateDtoExample>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<DriverLicenseImageDtoExample>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<RentalDtoExample>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<RentalReturnResultDtoExample>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<RentalReturnDtoExample>();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddApiVersioningConfiguration();

builder.Services.AddCachingConfiguration();

builder.Services.AddRateLimitingConfiguration(builder.Configuration);

builder.Services.AddHostedService<DeliveryProcessingService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BikeRentalDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseProblemDetails();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseIpRateLimiting();
app.UseMiddleware<ExceptionMiddleware>();


app.MapControllers();
app.MapHealthChecks("/health");

app.Run();