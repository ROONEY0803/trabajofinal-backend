namespace CountryRiskAI.API.Models.DTOs;

public class EconomicSummaryDto
{
    public IndicatorSeriesStats InflationStats { get; set; } = new();
    public IndicatorSeriesStats GdpGrowthStats { get; set; } = new();
    public IndicatorSeriesStats UnemploymentStats { get; set; } = new();
    public double GlobalGdpGrowth { get; set; }
    public double GlobalInflation { get; set; }
    
    // Propiedades legacy para compatibilidad
    public double AverageInflation => InflationStats.Average;
    public string InflationTrend => InflationStats.Trend > 0.5 ? "subiendo" : InflationStats.Trend < -0.5 ? "bajando" : "estable";
    public double AverageGdpGrowth => GdpGrowthStats.Average;
    public string GdpGrowthTrend => GdpGrowthStats.Trend > 0.5 ? "subiendo" : GdpGrowthStats.Trend < -0.5 ? "bajando" : "estable";
    public double? LastUnemploymentRate => UnemploymentStats.Last;
    public string UnemploymentTrend => UnemploymentStats.Trend > 0.5 ? "subiendo" : UnemploymentStats.Trend < -0.5 ? "bajando" : "estable";
    public string ComparisonWithGlobal { get; set; } = string.Empty;
}
