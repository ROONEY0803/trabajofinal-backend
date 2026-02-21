using Microsoft.AspNetCore.Mvc;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IRiskAnalysisService _riskAnalysisService;
    private readonly IInvestmentAnalysisService _investmentAnalysisService;
    private readonly ILogger<AnalysisController> _logger;

    public AnalysisController(
        IRiskAnalysisService riskAnalysisService,
        IInvestmentAnalysisService investmentAnalysisService,
        ILogger<AnalysisController> logger)
    {
        _riskAnalysisService = riskAnalysisService;
        _investmentAnalysisService = investmentAnalysisService;
        _logger = logger;
    }

    [HttpPost("risk")]
    public async Task<ActionResult<ApiResponse<RiskResponseDto>>> AnalyzeRisk(
        [FromBody] RiskRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<RiskResponseDto>
            {
                Error = "El nombre del país es requerido."
            });
        }

        try
        {
            var result = await _riskAnalysisService.AnalyzeRiskAsync(request.CountryName);
            return Ok(new ApiResponse<RiskResponseDto> { Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<RiskResponseDto>
            {
                Error = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<RiskResponseDto>
            {
                Error = ex.Message
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new ApiResponse<RiskResponseDto>
            {
                Error = "Error al consultar el servicio de IA."
            });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new ApiResponse<RiskResponseDto>
            {
                Error = "Timeout al consultar el servicio de IA."
            });
        }
    }

    [HttpPost("investment-outlook")]
    public async Task<ActionResult<ApiResponse<InvestmentOutlookDto>>> AnalyzeInvestment(
        [FromBody] InvestmentRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<InvestmentOutlookDto>
            {
                Error = "El nombre del país es requerido y el monto debe estar entre $1,000 y $10,000,000 USD."
            });
        }

        try
        {
            var result = await _investmentAnalysisService.AnalyzeInvestmentAsync(
                request.CountryName,
                request.PrincipalUsd);
            return Ok(new ApiResponse<InvestmentOutlookDto> { Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<InvestmentOutlookDto>
            {
                Error = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<InvestmentOutlookDto>
            {
                Error = ex.Message
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new ApiResponse<InvestmentOutlookDto>
            {
                Error = "Error al consultar servicios externos."
            });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new ApiResponse<InvestmentOutlookDto>
            {
                Error = "Timeout al consultar servicios externos."
            });
        }
    }
}
