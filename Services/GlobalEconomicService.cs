using CountryRiskAI.API.Clients;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Services;

public class GlobalEconomicService : IGlobalEconomicService
{
    private readonly WorldBankClient _worldBankClient;
    private readonly ILogger<GlobalEconomicService> _logger;

    public GlobalEconomicService(
        WorldBankClient worldBankClient,
        ILogger<GlobalEconomicService> logger)
    {
        _worldBankClient = worldBankClient;
        _logger = logger;
    }

    public async Task<(double GlobalGdpGrowth, double GlobalInflation)> GetGlobalEconomicContextAsync()
    {
        // Usar solo World Bank (WLD) para datos globales
        // Reducir rango a 5 años para evitar timeout
        var currentYear = DateTime.Now.Year;
        var startYear = currentYear - 5;

        // Obtener series históricas para calcular promedio
        var gdpSeriesTask = _worldBankClient.GetIndicatorAsync("WLD", "NY.GDP.MKTP.KD.ZG", startYear, currentYear);
        
        // Intentar primero con CPI, si falla usar GDP deflator como alternativa
        var inflationSeriesTask = _worldBankClient.GetIndicatorAsync("WLD", "FP.CPI.TOTL.ZG", startYear, currentYear);
        var inflationDeflatorTask = _worldBankClient.GetIndicatorAsync("WLD", "NY.GDP.DEFL.KD.ZG", startYear, currentYear);

        await Task.WhenAll(gdpSeriesTask, inflationSeriesTask, inflationDeflatorTask);

        var gdpSeries = await gdpSeriesTask;
        var inflationSeries = await inflationSeriesTask;
        var inflationDeflatorSeries = await inflationDeflatorTask;

        // Calcular GDP Growth
        double globalGdpGrowth = 3.0;
        if (gdpSeries.Any())
        {
            globalGdpGrowth = gdpSeries
                .Where(x => x.Value.HasValue)
                .OrderByDescending(x => x.Year)
                .Take(5)
                .Select(x => x.Value!.Value)
                .DefaultIfEmpty(3.0)
                .Average();
        }
        else
        {
            _logger.LogWarning("No se pudo obtener GDP global desde World Bank. Usando valor por defecto 3.0%");
        }

        // Calcular Inflation - usar CPI si está disponible, sino usar GDP deflator, sino valor por defecto
        double globalInflation = 3.5;
        if (inflationSeries.Any())
        {
            globalInflation = inflationSeries
                .Where(x => x.Value.HasValue)
                .OrderByDescending(x => x.Year)
                .Take(5)
                .Select(x => x.Value!.Value)
                .DefaultIfEmpty(3.5)
                .Average();
            _logger.LogInformation("Inflación global obtenida desde CPI (FP.CPI.TOTL.ZG)");
        }
        else if (inflationDeflatorSeries.Any())
        {
            globalInflation = inflationDeflatorSeries
                .Where(x => x.Value.HasValue)
                .OrderByDescending(x => x.Year)
                .Take(5)
                .Select(x => x.Value!.Value)
                .DefaultIfEmpty(3.5)
                .Average();
            _logger.LogInformation("Inflación global obtenida desde GDP Deflator (NY.GDP.DEFL.KD.ZG) como alternativa");
        }
        else
        {
            _logger.LogWarning(
                "No se pudo obtener inflación global desde World Bank (ni CPI ni GDP Deflator). Usando valor por defecto 3.5%");
        }

        _logger.LogInformation(
            "Datos globales desde World Bank - GDP: {Gdp}%, Inflation: {Infl}%",
            globalGdpGrowth,
            globalInflation);

        return (globalGdpGrowth, globalInflation);
    }
}
