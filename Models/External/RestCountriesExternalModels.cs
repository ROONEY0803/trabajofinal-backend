using System.Text.Json.Serialization;

namespace CountryRiskAI.API.Models.External;

public class RestCountriesResponse
{
    [JsonPropertyName("name")]
    public CountryName? Name { get; set; }

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("currencies")]
    public Dictionary<string, CurrencyInfo>? Currencies { get; set; }

    [JsonPropertyName("area")]
    public double Area { get; set; }

    [JsonPropertyName("cca3")]
    public string? Cca3 { get; set; }
}

public class CountryName
{
    [JsonPropertyName("common")]
    public string? Common { get; set; }
}

public class CurrencyInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}
