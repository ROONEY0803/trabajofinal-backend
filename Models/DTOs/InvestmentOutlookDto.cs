namespace CountryRiskAI.API.Models.DTOs;

public class InvestmentOutlookDto
{
    public string Country { get; set; } = string.Empty;
    public decimal PrincipalUsd { get; set; }
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public RoiAnnualDto RoiAnnual { get; set; } = new();
    public OutlookProjectionDto Projection { get; set; } = new();
    public string? Insight { get; set; }
    public string Note { get; set; } = "Nota: Este análisis corresponde a una evaluación macroeconómica general del país (bonos soberanos, mercado accionario, ETF o inversión productiva agregada). Las proyecciones son estimaciones basadas en datos históricos macroeconómicos y contexto global. No constituyen asesoría financiera ni garantizan resultados futuros.";
    public EconomicChartsDto? EconomicCharts { get; set; }
}

public class EconomicChartsDto
{
    public List<EconomicIndicatorDto> Inflation { get; set; } = new();
    public List<EconomicIndicatorDto> GdpGrowth { get; set; } = new();
    public List<EconomicIndicatorDto> Unemployment { get; set; } = new();
    public List<EconomicIndicatorDto> PublicDebt { get; set; } = new();
}

public class RoiAnnualDto
{
    public double Min { get; set; }
    public double Base { get; set; }
    public double Max { get; set; }
}

public class OutlookProjectionDto
{
    public YearProjectionDto Y1 { get; set; } = new();
    public YearProjectionDto Y3 { get; set; } = new();
    public YearProjectionDto Y5 { get; set; } = new();
}

public class YearProjectionDto
{
    public decimal FinalBase { get; set; }
    public string Label { get; set; } = string.Empty;
}
