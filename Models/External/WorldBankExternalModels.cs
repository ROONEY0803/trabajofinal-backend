using System.Text.Json.Serialization;

namespace CountryRiskAI.API.Models.External;

public class WorldBankResponse
{
    [JsonPropertyName("indicator")]
    public WorldBankIndicator? Indicator { get; set; }

    [JsonPropertyName("country")]
    public WorldBankCountry? Country { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }
}

public class WorldBankIndicator
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class WorldBankCountry
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class WorldBankApiResponse
{
    [JsonPropertyName("indicator")]
    public List<WorldBankIndicator>? Indicator { get; set; }

    [JsonPropertyName("country")]
    public List<WorldBankCountry>? Country { get; set; }

    [JsonPropertyName("source")]
    public List<object>? Source { get; set; }

    [JsonPropertyName("countryiso3code")]
    public string? CountryIso3Code { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("obs_status")]
    public string? ObsStatus { get; set; }

    [JsonPropertyName("decimal")]
    public int Decimal { get; set; }
}
