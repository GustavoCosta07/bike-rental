namespace BikeRental.Application.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] fileContent, string fileName, string[] allowedExtensions);
    Task<byte[]> GetFileAsync(string path);
    Task DeleteFileAsync(string path);
}