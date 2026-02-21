using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Helpers;

public static class EconomicCalculator
{
    public static EconomicSummaryDto CalculateSummary(
        EconomicDataDto economicData,
        double globalGdpGrowth,
        double globalInflation)
    {
        var summary = new EconomicSummaryDto
        {
            GlobalGdpGrowth = globalGdpGrowth,
            GlobalInflation = globalInflation
        };

        // Calcular estadísticas para cada indicador
        summary.InflationStats = StatsCalculator.CalculateStats(economicData.Inflation);
        summary.GdpGrowthStats = StatsCalculator.CalculateStats(economicData.GdpGrowth);
        summary.UnemploymentStats = StatsCalculator.CalculateStats(economicData.Unemployment);

        // Comparación con economía global
        summary.ComparisonWithGlobal = CompareWithGlobal(
            summary.AverageGdpGrowth,
            summary.AverageInflation,
            globalGdpGrowth,
            globalInflation);

        return summary;
    }

    private static string CompareWithGlobal(
        double countryGdpGrowth,
        double countryInflation,
        double globalGdpGrowth,
        double globalInflation)
    {
        var gdpComparison = countryGdpGrowth > globalGdpGrowth ? "superior" : "inferior";
        var inflationComparison = countryInflation < globalInflation ? "menor" : "mayor";

        return $"Crecimiento PIB {gdpComparison} al global ({countryGdpGrowth:F2}% vs {globalGdpGrowth:F2}%). " +
               $"Inflación {inflationComparison} que la global ({countryInflation:F2}% vs {globalInflation:F2}%).";
    }
}
