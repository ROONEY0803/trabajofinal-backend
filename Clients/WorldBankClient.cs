using System.Net.Http.Json;
using System.Text.Json;
using CountryRiskAI.API.Models.DTOs;
using CountryRiskAI.API.Models.External;

namespace CountryRiskAI.API.Clients;

public class WorldBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorldBankClient> _logger;

    public WorldBankClient(
        HttpClient httpClient,
        ILogger<WorldBankClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EconomicIndicatorDto>> GetIndicatorAsync(
        string countryCode,
        string indicatorCode,
        int startYear,
        int endYear)
    {
        try
        {
            var url = $"/v2/country/{countryCode}/indicator/{indicatorCode}?date={startYear}:{endYear}&format=json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Error al obtener indicador {Indicator} para {Country}: {StatusCode}",
                    indicatorCode,
                    countryCode,
                    response.StatusCode);
                return new List<EconomicIndicatorDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() < 2)
            {
                return new List<EconomicIndicatorDto>();
            }

            var dataArray = root[1];
            var indicators = new List<EconomicIndicatorDto>();

            foreach (var item in dataArray.EnumerateArray())
            {
                if (item.TryGetProperty("value", out var valueElement) &&
                    valueElement.ValueKind != JsonValueKind.Null &&
                    item.TryGetProperty("date", out var dateElement))
                {
                    if (int.TryParse(dateElement.GetString(), out var year) &&
                        valueElement.TryGetDouble(out var value))
                    {
                        indicators.Add(new EconomicIndicatorDto
                        {
                            Year = year,
                            Value = value
                        });
                    }
                }
            }

            return indicators.OrderBy(x => x.Year).ToList();
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex.InnerException is TaskCanceledException)
            {
                _logger.LogWarning(
                    "Timeout al consultar World Bank para {Country} - {Indicator}. Usando valores por defecto.",
                    countryCode,
                    indicatorCode);
            }
            else
            {
                _logger.LogError(ex, "Error al consultar World Bank para {Country}", countryCode);
            }
            return new List<EconomicIndicatorDto>();
        }
    }

    public async Task<double?> GetGlobalIndicatorAsync(string indicatorCode, int year)
    {
        try
        {
            // Intentar con el año actual y años anteriores si no hay datos
            var yearsToTry = new[] { year, year - 1, year - 2, year - 3 };
            
            foreach (var tryYear in yearsToTry)
            {
                var url = $"/v2/country/WLD/indicator/{indicatorCode}?date={tryYear}&format=json";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    continue;
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                // Verificar que es un array y tiene al menos 2 elementos
                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() < 2)
                {
                    continue;
                }

                // Verificar que el segundo elemento existe y es un array
                var dataArray = root[1];
                if (dataArray.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                // Buscar el primer valor válido
                foreach (var item in dataArray.EnumerateArray())
                {
                    if (item.TryGetProperty("value", out var valueElement) &&
                        valueElement.ValueKind != JsonValueKind.Null &&
                        valueElement.TryGetDouble(out var value))
                    {
                        _logger.LogInformation(
                            "Indicador global {Indicator} obtenido desde World Bank para año {Year}: {Value}",
                            indicatorCode,
                            tryYear,
                            value);
                        return value;
                    }
                }
            }

            _logger.LogWarning(
                "No se encontraron datos para indicador global {Indicator} en World Bank",
                indicatorCode);
            return null;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex.InnerException is TaskCanceledException)
            {
                _logger.LogWarning(
                    "Timeout al consultar World Bank global para {Indicator}. Usando valores por defecto.",
                    indicatorCode);
            }
            else
            {
                _logger.LogError(ex, "Error al consultar World Bank global para {Indicator}", indicatorCode);
            }
            return null;
        }
    }
}
