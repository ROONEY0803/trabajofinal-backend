using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Helpers;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Services;

public class InvestmentAnalysisService : IInvestmentAnalysisService
{
    private readonly ICountryService _countryService;
    private readonly IEconomicDataService _economicDataService;
    private readonly IGlobalEconomicService _globalEconomicService;
    private readonly IGeminiClient _geminiClient;
    private readonly ILogger<InvestmentAnalysisService> _logger;

    public InvestmentAnalysisService(
        ICountryService countryService,
        IEconomicDataService economicDataService,
        IGlobalEconomicService globalEconomicService,
        IGeminiClient geminiClient,
        ILogger<InvestmentAnalysisService> logger)
    {
        _countryService = countryService;
        _economicDataService = economicDataService;
        _globalEconomicService = globalEconomicService;
        _geminiClient = geminiClient;
        _logger = logger;
    }

    public async Task<InvestmentOutlookDto> AnalyzeInvestmentAsync(
        string countryName,
        decimal principalUsd)
    {
        var country = await _countryService.GetCountryAsync(countryName);

        if (country == null)
        {
            throw new KeyNotFoundException($"País '{countryName}' no encontrado.");
        }

        if (string.IsNullOrEmpty(country.CountryCode))
        {
            throw new InvalidOperationException($"Código de país no disponible para '{countryName}'.");
        }

        var economicDataTask = _economicDataService.GetEconomicDataAsync(country.CountryCode);
        var globalContextTask = _globalEconomicService.GetGlobalEconomicContextAsync();

        await Task.WhenAll(economicDataTask, globalContextTask);

        var economicData = await economicDataTask;
        var (globalGdpGrowth, globalInflation) = await globalContextTask;

        var economicSummary = EconomicCalculator.CalculateSummary(
            economicData,
            globalGdpGrowth,
            globalInflation);

        var outlook = InvestmentCalculator.CalculateInvestmentOutlook(
            economicSummary,
            country.Name,
            principalUsd);

        outlook.EconomicCharts = new EconomicChartsDto
        {
            Inflation = economicData.Inflation.OrderBy(x => x.Year).ToList(),
            GdpGrowth = economicData.GdpGrowth.OrderBy(x => x.Year).ToList(),
            Unemployment = economicData.Unemployment.OrderBy(x => x.Year).ToList(),
            PublicDebt = economicData.PublicDebt.OrderBy(x => x.Year).ToList()
        };

        // Obtener interpretación de IA
        var interpretationPrompt = PromptBuilder.BuildInvestmentInterpretationPrompt(outlook);
        var aiInterpretation = await _geminiClient.GenerateTextAsync(interpretationPrompt);
        outlook.Insight = CleanInsightText(aiInterpretation);

        return outlook;
    }

    private static string? CleanInsightText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        // Reemplazar saltos de línea con espacios
        var cleaned = text
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");

        // Eliminar espacios múltiples
        while (cleaned.Contains("  "))
        {
            cleaned = cleaned.Replace("  ", " ");
        }

        return cleaned.Trim();
    }
}
