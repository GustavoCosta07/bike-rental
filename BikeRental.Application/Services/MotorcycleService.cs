using System.ComponentModel.DataAnnotations;
using BikeRental.Application.Mapping;
using BikeRental.Application.Models;
using BikeRental.Domain.Entities;
using BikeRental.Domain.Events;
using BikeRental.Domain.Repositories;
using FluentValidation;
using MassTransit;

namespace BikeRental.Application.Services;

public class MotorcycleService
{
    private readonly IMotorcycleRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IValidator<MotorcycleCreateDto> _validator;

    public MotorcycleService(IMotorcycleRepository repository, IPublishEndpoint publishEndpoint, IValidator<MotorcycleCreateDto> validator)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _validator = validator;
    }

    public async Task<MotorcycleDto> CreateMotorcycleAsync(MotorcycleCreateDto createDto)
    {
        var validationResult = await _validator.ValidateAsync(createDto);
        if(!validationResult.IsValid)
        {
            throw new InvalidOperationException("Input is not valid");
        }
        if (await _repository.LicensePlateExistsAsync(createDto.LicensePlate))
        {
            throw new InvalidOperationException("License plate already exists");
        }

        var motorcycle = new Motorcycle
        {
            Year = createDto.Year,
            Model = createDto.Model,
            LicensePlate = createDto.LicensePlate
        };

        var created = await _repository.AddAsync(motorcycle);

        await _publishEndpoint.Publish(new MotorcycleRegisteredEvent(
            created.Id,
            created.Year,
            created.LicensePlate));

        return created.MapToDto();
    }


    public async Task<IEnumerable<MotorcycleDto>> GetAllMotorcyclesAsync(string? licensePlate)
    {
        var motorcycles = await _repository.GetAllAsync(licensePlate);
        return motorcycles.Select(x => x.MapToDto());
    }

    public async Task<MotorcycleDto?> GetMotorcycleByIdAsync(Guid id)
    {
        var motorcycle = await _repository.GetByIdAsync(id);
        return motorcycle is not null ? motorcycle.MapToDto() : null;
    }

    public async Task<MotorcycleDto?> UpdateMotorcycleAsync(Guid id, MotorcycleUpdateDto updateDto)
    {
        var motorcycle = await _repository.GetByIdAsync(id);
        if (motorcycle is null) return null;

        motorcycle.LicensePlate = updateDto.LicensePlate;
        await _repository.UpdateAsync(motorcycle);

        return motorcycle.MapToDto();
    }

    public async Task<bool> DeleteMotorcycleAsync(Guid id)
    {
        var motorcycle = await _repository.GetByIdAsync(id);
        if (motorcycle is null) return false;

        await _repository.DeleteAsync(motorcycle);
        return true;
    }
}