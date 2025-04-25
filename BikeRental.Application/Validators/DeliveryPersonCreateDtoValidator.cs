using BikeRental.Application.Models;
using BikeRental.Domain;
using FluentValidation;

namespace BikeRental.Application.Validators;

public class DeliveryPersonCreateDtoValidator : AbstractValidator<DeliveryPersonCreateDto>
{
    public DeliveryPersonCreateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("ID must be a valid GUID");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .Matches(@"^\d{14}$")
            .WithMessage("Tax ID must be 14 digits");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow)
            .GreaterThan(DateTime.UtcNow.AddYears(-100));

        RuleFor(x => x.DriverLicenseNumber)
            .NotEmpty()
            .Matches(@"^\d{11}$")
            .WithMessage("Driver license number must be 11 digits");

        RuleFor(x => x.DriverLicenseType)
            .Must(type => Enum.TryParse<LicenseType>(type, true, out _))
            .WithMessage("Driver license type must be A, B, or AB");

        RuleFor(x => x.DriverLicenseImageBase64)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("Driver license image must be a valid base64 string")
            .Must(BeValidImageFormat)
            .WithMessage("Driver license image must be in PNG or BMP format");
    }

    private bool BeValidBase64(string base64)
    {
        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool BeValidImageFormat(string base64)
    {
        try
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            byte[] pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            byte[] bmpSignature = new byte[] { 0x42, 0x4D };

            if (imageBytes.Length >= pngSignature.Length && imageBytes.Take(pngSignature.Length).SequenceEqual(pngSignature))
            {
                return true;
            }
            if (imageBytes.Length >= bmpSignature.Length && imageBytes.Take(bmpSignature.Length).SequenceEqual(bmpSignature))
            {
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}