using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Services.Interfaces;

public interface IRiskAnalysisService
{
    Task<RiskResponseDto> AnalyzeRiskAsync(string countryName);
}
