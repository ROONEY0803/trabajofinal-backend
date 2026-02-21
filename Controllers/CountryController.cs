using Microsoft.AspNetCore.Mvc;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly ILogger<CountryController> _logger;

    public CountryController(
        ICountryService countryService,
        ILogger<CountryController> logger)
    {
        _countryService = countryService;
        _logger = logger;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<ApiResponse<CountryDto>>> GetCountry(string name)
    {
        try
        {
            var country = await _countryService.GetCountryAsync(name);

            if (country == null)
            {
                return NotFound(new ApiResponse<CountryDto>
                {
                    Error = $"País '{name}' no encontrado."
                });
            }

            return Ok(new ApiResponse<CountryDto> { Data = country });
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new ApiResponse<CountryDto>
            {
                Error = "Error al consultar la API de países."
            });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new ApiResponse<CountryDto>
            {
                Error = "Timeout al consultar la API externa."
            });
        }
    }
}
