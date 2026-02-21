using Microsoft.AspNetCore.Mvc;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly IGlobalEconomicService _globalEconomicService;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(
        IGlobalEconomicService globalEconomicService,
        ILogger<DiagnosticsController> logger)
    {
        _globalEconomicService = globalEconomicService;
        _logger = logger;
    }

    [HttpGet("global")]
    public async Task<ActionResult<ApiResponse<object>>> GetGlobalData()
    {
        try
        {
            var (globalGdpGrowth, globalInflation) = await _globalEconomicService.GetGlobalEconomicContextAsync();

            return Ok(new ApiResponse<object>
            {
                Data = new
                {
                    sourceUsed = "World Bank (WLD)",
                    globalGdpGrowth,
                    globalInflation,
                    timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos globales");
            return StatusCode(500, new ApiResponse<object>
            {
                Error = "Error al obtener datos globales"
            });
        }
    }
}
