using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Helpers;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Services.Interfaces;

namespace CountryRiskAI.API.Services;

public class CountryService : ICountryService
{
    private readonly IRestCountriesClient _restCountriesClient;
    private readonly ILogger<CountryService> _logger;

    public CountryService(
        IRestCountriesClient restCountriesClient,
        ILogger<CountryService> logger)
    {
        _restCountriesClient = restCountriesClient;
        _logger = logger;
    }

    public async Task<CountryDto?> GetCountryAsync(string countryName)
    {
        // Intentar con diferentes variantes del nombre
        var variants = CountryNameTranslator.GetSearchVariants(countryName);
        
        _logger.LogInformation(
            "Buscando país '{Country}' con variantes: {Variants}",
            countryName,
            string.Join(", ", variants));

        foreach (var variant in variants)
        {
            var countryData = await _restCountriesClient.GetCountryAsync(variant);

            if (countryData != null)
            {
                _logger.LogInformation("País encontrado con variante: {Variant}", variant);
                var currency = countryData.Currencies?.Keys.FirstOrDefault() ?? string.Empty;

                return new CountryDto
                {
                    Name = countryData.Name?.Common ?? string.Empty,
                    Population = countryData.Population,
                    Region = countryData.Region ?? string.Empty,
                    Currency = currency,
                    Area = countryData.Area,
                    CountryCode = countryData.Cca3 ?? string.Empty
                };
            }
        }

        _logger.LogWarning("No se encontró el país '{Country}' con ninguna variante", countryName);
        return null;
    }
}
