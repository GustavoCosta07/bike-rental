using BikeRental.Application.Models;
using BikeRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/rentals-controller")]
public class RentalsController : ControllerBase
{
    private readonly RentalService _service;

    public RentalsController(RentalService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RentalDto createDto)
    {
        var created = await _service.CreateRentalAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rental = await _service.GetRentalByIdAsync(id);
        return rental != null ? Ok(rental) : throw new KeyNotFoundException("Locação não encontrada");
    }

    [HttpPut("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id, [FromBody] RentalReturnDto returnDto)
    {
        var result = await _service.ReturnRentalAsync(id, returnDto);
        return Ok(result);
    }
}