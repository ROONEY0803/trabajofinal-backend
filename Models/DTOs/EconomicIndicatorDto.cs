namespace CountryRiskAI.API.Models.DTOs;

public class EconomicIndicatorDto
{
    public int Year { get; set; }
    public double? Value { get; set; }
}

public class EconomicDataDto
{
    public List<EconomicIndicatorDto> Inflation { get; set; } = new();
    public List<EconomicIndicatorDto> GdpGrowth { get; set; } = new();
    public List<EconomicIndicatorDto> Unemployment { get; set; } = new();
    public List<EconomicIndicatorDto> PublicDebt { get; set; } = new();
}
