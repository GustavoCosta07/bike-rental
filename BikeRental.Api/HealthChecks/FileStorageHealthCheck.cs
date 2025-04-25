using System.Text;
using BikeRental.Application.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BikeRental.Api.HealthChecks;

public class FileStorageHealthCheck : IHealthCheck
{
    private readonly IFileStorageService _fileStorage;

    public FileStorageHealthCheck(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testContent = Encoding.UTF8.GetBytes("healthcheck");
            var fileName = $"healthcheck_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
            var path = await _fileStorage.SaveFileAsync(testContent, fileName, new[] { ".txt" });
            await _fileStorage.GetFileAsync(path);
            await _fileStorage.DeleteFileAsync(path);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}