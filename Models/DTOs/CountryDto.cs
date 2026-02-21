namespace CountryRiskAI.API.Models.DTOs;

public class CountryDto
{
    public string Name { get; set; } = string.Empty;
    public long Population { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public double Area { get; set; }
    public string CountryCode { get; set; } = string.Empty;
}
