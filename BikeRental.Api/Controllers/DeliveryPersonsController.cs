using BikeRental.Application.Models;
using BikeRental.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryPersonsController : ControllerBase
{
    private readonly DeliveryPersonService _service;

    public DeliveryPersonsController(DeliveryPersonService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeliveryPersonCreateDto createDto)
    {
        try
        {
            var created = await _service.CreateDeliveryPersonAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponseDto("Invalid data"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponseDto(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var deliveryPerson = await _service.GetByIdAsync(id);
            return deliveryPerson != null ? Ok(deliveryPerson) : NotFound(new ErrorResponseDto("Delivery person not found"));
        }
        catch
        {
            return BadRequest(new ErrorResponseDto("Invalid data"));
        }
    }

    [HttpPost("{id:guid}/driver-license-image")]
    public async Task<IActionResult> UpdateDriverLicenseImage(Guid id, [FromBody] DriverLicenseImageDto imageDto)
    {
        try
        {
            await _service.UpdateDriverLicenseImageAsync(id, imageDto);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponseDto("Invalid data"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponseDto("Delivery person not found"));
        }
    }
}