using System.Diagnostics;

namespace BikeRental.Tests.Integration;

public static class LocalStackSetup
{
    public static async Task StartLocalStack()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "run -d -p 4566:4566 localstack/localstack:3.4.0",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };
        process.Start();
        await process.WaitForExitAsync();
    }
}