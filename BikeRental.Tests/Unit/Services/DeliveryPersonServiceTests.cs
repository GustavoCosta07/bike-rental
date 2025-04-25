using BikeRental.Application.Models;
using BikeRental.Application.Services;
using BikeRental.Domain;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace BikeRental.Tests.Unit.Services;

public class DeliveryPersonServiceTests
{
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IValidator<DeliveryPersonCreateDto>> _createValidatorMock;
    private readonly Mock<IValidator<DriverLicenseImageDto>> _imageValidatorMock;
    private readonly DeliveryPersonService _service;

    public DeliveryPersonServiceTests()
    {
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _createValidatorMock = new Mock<IValidator<DeliveryPersonCreateDto>>();
        _imageValidatorMock = new Mock<IValidator<DriverLicenseImageDto>>();
        _service = new DeliveryPersonService(
            _deliveryPersonRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _createValidatorMock.Object,
            _imageValidatorMock.Object);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_ValidDto_CreatesDeliveryPerson()
    {
        var createDto = new DeliveryPersonCreateDto(
            Id: Guid.NewGuid().ToString(),
            Name: "João da Silva",
            TaxId: "12345678901234",
            BirthDate: new DateTime(1990, 1, 1),
            DriverLicenseNumber: "12345678901",
            DriverLicenseType: "A",
            DriverLicenseImageBase64: Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })
        );
        var deliveryPerson = new DeliveryPerson
        {
            Id = Guid.Parse(createDto.Id),
            Name = createDto.Name,
            CNPJ = createDto.TaxId,
            BirthDate = createDto.BirthDate,
            DriverLicenseNumber = createDto.DriverLicenseNumber,
            DriverLicenseType = LicenseType.A,
            DriverLicenseImageUrl = "/Storage/test.png"
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _deliveryPersonRepositoryMock.Setup(r => r.CnpjExistsAsync(createDto.TaxId))
            .ReturnsAsync(false);
        _deliveryPersonRepositoryMock.Setup(r => r.DriverLicenseNumberExistsAsync(createDto.DriverLicenseNumber))
            .ReturnsAsync(false);
        _fileStorageServiceMock.Setup(s => s.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync("/Storage/test.png");
        _deliveryPersonRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DeliveryPerson>()))
            .ReturnsAsync(deliveryPerson);

        var result = await _service.CreateDeliveryPersonAsync(createDto);

        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.TaxId.Should().Be(createDto.TaxId);
        result.DriverLicenseImageUrl.Should().Be("/Storage/test.png");
    }

    [Fact]
    public async Task UpdateDriverLicenseImageAsync_ValidImage_UpdatesImage()
    {
        var id = Guid.NewGuid();
        var imageDto = new DriverLicenseImageDto(Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }));
        var deliveryPerson = new DeliveryPerson
        {
            Id = id,
            Name = "João",
            CNPJ = "12345678901234",
            BirthDate = new DateTime(1990, 1, 1),
            DriverLicenseNumber = "12345678901",
            DriverLicenseType = LicenseType.A,
            DriverLicenseImageUrl = "/Storage/old.png"
        };

        _imageValidatorMock.Setup(v => v.ValidateAsync(imageDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _deliveryPersonRepositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(deliveryPerson);
        _fileStorageServiceMock.Setup(s => s.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync("/Storage/new.png");

        await _service.UpdateDriverLicenseImageAsync(id, imageDto);

        _deliveryPersonRepositoryMock.Verify(r => r.UpdateAsync(It.Is<DeliveryPerson>(d => d.DriverLicenseImageUrl == "/Storage/new.png")), Times.Once());
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_InvalidImageFormat_ThrowsInvalidOperationException()
    {
        var createDto = new DeliveryPersonCreateDto(
            Id: Guid.NewGuid().ToString(),
            Name: "João da Silva",
            TaxId: "12345678901234",
            BirthDate: new DateTime(1990, 1, 1),
            DriverLicenseNumber: "12345678901",
            DriverLicenseType: "A",
            DriverLicenseImageBase64: Convert.ToBase64String(new byte[] { 0xFF, 0xD8, 0xFF })
        );

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _deliveryPersonRepositoryMock.Setup(r => r.CnpjExistsAsync(createDto.TaxId))
            .ReturnsAsync(false);
        _deliveryPersonRepositoryMock.Setup(r => r.DriverLicenseNumberExistsAsync(createDto.DriverLicenseNumber))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeliveryPersonAsync(createDto));
    }
}