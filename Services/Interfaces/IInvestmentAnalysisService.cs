using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Services.Interfaces;

public interface IInvestmentAnalysisService
{
    Task<InvestmentOutlookDto> AnalyzeInvestmentAsync(string countryName, decimal principalUsd);
}
