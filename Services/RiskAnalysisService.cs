using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Helpers;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Services;

public class RiskAnalysisService : IRiskAnalysisService
{
    private readonly ICountryService _countryService;
    private readonly IEconomicDataService _economicDataService;
    private readonly IGlobalEconomicService _globalEconomicService;
    private readonly IGeminiClient _geminiClient;
    private readonly ILogger<RiskAnalysisService> _logger;

    public RiskAnalysisService(
        ICountryService countryService,
        IEconomicDataService economicDataService,
        IGlobalEconomicService globalEconomicService,
        IGeminiClient geminiClient,
        ILogger<RiskAnalysisService> logger)
    {
        _countryService = countryService;
        _economicDataService = economicDataService;
        _globalEconomicService = globalEconomicService;
        _geminiClient = geminiClient;
        _logger = logger;
    }

    public async Task<RiskResponseDto> AnalyzeRiskAsync(string countryName)
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

        var prompt = PromptBuilder.BuildRiskAnalysisPrompt(country, economicSummary);
        var aiResponse = await _geminiClient.GenerateTextAsync(prompt);

        var (score, recommendation) = RiskParser.ParseAiResponse(aiResponse);
        var riskLevel = RiskRules.GetRiskLevel(score);

        return new RiskResponseDto
        {
            Country = country.Name,
            RiskScore = score,
            RiskLevel = riskLevel,
            Recommendation = recommendation
        };
    }
}
