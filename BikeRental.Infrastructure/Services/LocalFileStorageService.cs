using BikeRental.Application.Services;

namespace BikeRental.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public LocalFileStorageService()
    {
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string[] allowedExtensions)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Invalid file format. Allowed formats: {string.Join(", ", allowedExtensions)}");
        }

        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, fileContent);
        return $"/Storage/{fileName}";
    }

    public async Task<byte[]> GetFileAsync(string path)
    {
        var fullPath = Path.Combine(_storagePath, Path.GetFileName(path.TrimStart('/')));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", fullPath);
        }

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task DeleteFileAsync(string path)
    {
        var fullPath = Path.Combine(_storagePath, Path.GetFileName(path.TrimStart('/')));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            await Task.CompletedTask;
        }
    }
}