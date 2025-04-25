using BikeRental.Application.Models;
using BikeRental.Application.Services;
using BikeRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/motorcycles")]
public class MotorcyclesController : ControllerBase
{
    private readonly MotorcycleService _service;


    public MotorcyclesController(MotorcycleService service, BikeRentalDbContext context)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? licensePlate)
    {
        var motorcycles = await _service.GetAllMotorcyclesAsync(licensePlate);
        return Ok(motorcycles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var motorcycle = await _service.GetMotorcycleByIdAsync(id);
        return motorcycle is not null ? Ok(motorcycle) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MotorcycleCreateDto createDto)
    {
        var created = await _service.CreateMotorcycleAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MotorcycleUpdateDto updateDto)
    {
        var updated = await _service.UpdateMotorcycleAsync(id, updateDto);
        return updated is not null ? Ok(updated) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteMotorcycleAsync(id);
        return result ? NoContent() : NotFound();
    }

}