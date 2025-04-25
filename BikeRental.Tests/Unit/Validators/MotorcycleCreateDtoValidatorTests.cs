using BikeRental.Application.Models;
using BikeRental.Application.Validators;
using FluentAssertions;

namespace BikeRental.Tests.Unit.Validators;

public class MotorcycleCreateDtoValidatorTests
{
    private readonly MotorcycleCreateDtoValidator _validator;

    public MotorcycleCreateDtoValidatorTests()
    {
        _validator = new MotorcycleCreateDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_ReturnsNoErrors()
    {
        var dto = new MotorcycleCreateDto(2023, "Honda CB 500", "ABC1D23");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidLicensePlate_ReturnsError()
    {
        var dto = new MotorcycleCreateDto(2023, "Honda CB 500", "INVALID");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("License plate must be in the format AAA1A11"));
    }

    [Fact]
    public void Validate_YearOutOfRange_ReturnsError()
    {
        var dto = new MotorcycleCreateDto(1800, "Honda CB 500", "ABC1D23");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }
}