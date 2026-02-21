using CountryRiskAI.API.Clients;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Services;

public class EconomicDataService : IEconomicDataService
{
    private readonly WorldBankClient _worldBankClient;
    private readonly ILogger<EconomicDataService> _logger;

    public EconomicDataService(
        WorldBankClient worldBankClient,
        ILogger<EconomicDataService> logger)
    {
        _worldBankClient = worldBankClient;
        _logger = logger;
    }

    public async Task<EconomicDataDto> GetEconomicDataAsync(string countryCode)
    {
        var currentYear = DateTime.Now.Year;
        var startYear = currentYear - 10;

        var inflationTask = _worldBankClient.GetIndicatorAsync(
            countryCode, "FP.CPI.TOTL.ZG", startYear, currentYear);
        var gdpGrowthTask = _worldBankClient.GetIndicatorAsync(
            countryCode, "NY.GDP.MKTP.KD.ZG", startYear, currentYear);
        var unemploymentTask = _worldBankClient.GetIndicatorAsync(
            countryCode, "SL.UEM.TOTL.ZS", startYear, currentYear);
        var publicDebtTask = _worldBankClient.GetIndicatorAsync(
            countryCode, "GC.DOD.TOTL.GD.ZS", startYear, currentYear);

        await Task.WhenAll(inflationTask, gdpGrowthTask, unemploymentTask,
            publicDebtTask);

        return new EconomicDataDto
        {
            Inflation = await inflationTask,
            GdpGrowth = await gdpGrowthTask,
            Unemployment = await unemploymentTask,
            PublicDebt = await publicDebtTask
        };
    }
}
