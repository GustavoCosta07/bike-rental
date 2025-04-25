using BikeRental.Application.Models;
using BikeRental.Application.Services;
using BikeRental.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        try
        {
            var created = await _service.CreateRentalAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponseDto("Dados inválidos"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponseDto(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponseDto(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var rental = await _service.GetRentalByIdAsync(id);
            return rental != null ? Ok(rental) : NotFound(new ErrorResponseDto("Locação não encontrada"));
        }
        catch
        {
            return BadRequest(new ErrorResponseDto("Dados inválidos"));
        }
    }

    [HttpPut("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id, [FromBody] RentalReturnDto returnDto)
    {
        try
        {
            var result = await _service.ReturnRentalAsync(id, returnDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponseDto("Dados inválidos: " + string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponseDto(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponseDto(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto("Ocorreu um erro inesperado"));
        }
    }
}