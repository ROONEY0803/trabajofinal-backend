namespace CountryRiskAI.API.Models.DTOs;

public class IndicatorSeriesStats
{
    public double Average { get; set; }
    public double? Last { get; set; }
    public double Trend { get; set; }
    public double StdDev { get; set; }
    public int YearsUsed { get; set; }
    public bool IsValid => YearsUsed >= 5;
}
