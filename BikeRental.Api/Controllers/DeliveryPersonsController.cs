using BikeRental.Application.Models;
using BikeRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/delivery-persons")]
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
        var created = await _service.CreateDeliveryPersonAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var deliveryPerson = await _service.GetByIdAsync(id);
        return deliveryPerson != null ? Ok(deliveryPerson) : throw new KeyNotFoundException("Delivery person not found");
    }

    [HttpPost("{id:guid}/driver-license-image")]
    public async Task<IActionResult> UpdateDriverLicenseImage(Guid id, [FromBody] DriverLicenseImageDto imageDto)
    {
        await _service.UpdateDriverLicenseImageAsync(id, imageDto);
        return Ok();
    }
}