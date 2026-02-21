using System.Net.Http.Json;
using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Models.External;

namespace CountryRiskAI.API.Clients;

public class RestCountriesClient : IRestCountriesClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RestCountriesClient> _logger;

    public RestCountriesClient(
        HttpClient httpClient,
        ILogger<RestCountriesClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RestCountriesResponse?> GetCountryAsync(string countryName)
    {
        try
        {
            var url = $"/v3.1/name/{Uri.EscapeDataString(countryName)}?fullText=false";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Error al obtener país {Country}: {StatusCode}",
                    countryName,
                    response.StatusCode);
                return null;
            }

            var countries = await response.Content
                .ReadFromJsonAsync<List<RestCountriesResponse>>();

            return countries?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar REST Countries para {Country}", countryName);
            throw;
        }
    }
}
