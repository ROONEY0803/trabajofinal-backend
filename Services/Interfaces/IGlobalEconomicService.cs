namespace CountryRiskAI.API.Services.Interfaces;

public interface IGlobalEconomicService
{
    Task<(double GlobalGdpGrowth, double GlobalInflation)> GetGlobalEconomicContextAsync();
}
