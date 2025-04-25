using BikeRental.Application.Mapping;
using BikeRental.Application.Models;
using BikeRental.Domain;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using FluentValidation;

namespace BikeRental.Application.Services;

public class DeliveryPersonService
{
    private readonly IDeliveryPersonRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IValidator<DeliveryPersonCreateDto> _createValidator;
    private readonly IValidator<DriverLicenseImageDto> _imageValidator;

    public DeliveryPersonService(
        IDeliveryPersonRepository repository,
        IFileStorageService fileStorageService,
        IValidator<DeliveryPersonCreateDto> createValidator,
        IValidator<DriverLicenseImageDto> imageValidator)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _createValidator = createValidator;
        _imageValidator = imageValidator;
    }

    public async Task<DeliveryPersonDto> CreateDeliveryPersonAsync(DeliveryPersonCreateDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (await _repository.CnpjExistsAsync(createDto.TaxId))
        {
            throw new InvalidOperationException("Tax ID already exists");
        }

        if (await _repository.DriverLicenseNumberExistsAsync(createDto.DriverLicenseNumber))
        {
            throw new InvalidOperationException("Driver license number already exists");
        }

        if (!Enum.TryParse<LicenseType>(createDto.DriverLicenseType, true, out var licenseType))
        {
            throw new InvalidOperationException("Invalid driver license type");
        }

        byte[] imageBytes = Convert.FromBase64String(createDto.DriverLicenseImageBase64);
        string extension = DetectImageExtension(imageBytes);
        if (extension == null || !new[] { ".png", ".bmp" }.Contains(extension))
        {
            throw new InvalidOperationException("Driver license image must be in PNG or BMP format");
        }

        string fileName = $"{createDto.Id}_driver_license_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        string imageUrl = await _fileStorageService.SaveFileAsync(imageBytes, fileName, new[] { ".png", ".bmp" });

        var deliveryPerson = new DeliveryPerson
        {
            Id = Guid.Parse(createDto.Id),
            Name = createDto.Name,
            CNPJ = createDto.TaxId,
            BirthDate = createDto.BirthDate,
            DriverLicenseNumber = createDto.DriverLicenseNumber,
            DriverLicenseType = licenseType,
            DriverLicenseImageUrl = imageUrl
        };

        var created = await _repository.AddAsync(deliveryPerson);
        return created.MapToDto();
    }

    public async Task UpdateDriverLicenseImageAsync(Guid id, DriverLicenseImageDto imageDto)
    {
        var validationResult = await _imageValidator.ValidateAsync(imageDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var deliveryPerson = await _repository.GetByIdAsync(id);
        if (deliveryPerson == null)
        {
            throw new KeyNotFoundException("Delivery person not found");
        }

        byte[] imageBytes = Convert.FromBase64String(imageDto.DriverLicenseImageBase64);
        string extension = DetectImageExtension(imageBytes);
        if (extension == null || !new[] { ".png", ".bmp" }.Contains(extension))
        {
            throw new InvalidOperationException("Driver license image must be in PNG or BMP format");
        }

        string fileName = $"{id}_driver_license_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        string imageUrl = await _fileStorageService.SaveFileAsync(imageBytes, fileName, new[] { ".png", ".bmp" });

        deliveryPerson.DriverLicenseImageUrl = imageUrl;
        await _repository.UpdateAsync(deliveryPerson);
    }

    public async Task<DeliveryPersonDto?> GetByIdAsync(Guid id)
    {
        var deliveryPerson = await _repository.GetByIdAsync(id);
        return deliveryPerson?.MapToDto();
    }

    private string? DetectImageExtension(byte[] imageBytes)
    {
        byte[] pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        byte[] bmpSignature = new byte[] { 0x42, 0x4D };

        if (imageBytes.Length >= pngSignature.Length && imageBytes.Take(pngSignature.Length).SequenceEqual(pngSignature))
        {
            return ".png";
        }
        if (imageBytes.Length >= bmpSignature.Length && imageBytes.Take(bmpSignature.Length).SequenceEqual(bmpSignature))
        {
            return ".bmp";
        }

        return null;
    }
}