using System.Net.Http.Json;
using System.Text.Json;
using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Models.External;

namespace CountryRiskAI.API.Clients;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiClient> _logger;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiClient(
        HttpClient httpClient,
        ILogger<GeminiClient> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["ExternalApis:Gemini:ApiKey"] ?? string.Empty;
        _model = configuration["ExternalApis:Gemini:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<string?> GenerateTextAsync(string prompt)
    {
        try
        {
            var request = new GeminiRequest
            {
                Contents = new List<GeminiContent>
                {
                    new GeminiContent
                    {
                        Parts = new List<GeminiPart>
                        {
                            new GeminiPart { Text = prompt }
                        }
                    }
                }
            };

            // Usar v1beta con gemini-1.5-flash (modelo soportado)
            var url = $"/v1beta/models/{_model}:generateContent?key={Uri.EscapeDataString(_apiKey)}";
            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Error al generar texto con Gemini: {StatusCode}, {Error}",
                    response.StatusCode,
                    errorContent);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(content);

            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar Gemini");
            throw;
        }
    }
}
