using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Helpers;

public static class InvestmentCalculator
{
    public static InvestmentOutlookDto CalculateInvestmentOutlook(
        EconomicSummaryDto economicSummary,
        string countryName,
        decimal principalUsd)
    {
        // Validar que hay suficientes datos
        if (!economicSummary.InflationStats.IsValid ||
            !economicSummary.GdpGrowthStats.IsValid ||
            !economicSummary.UnemploymentStats.IsValid)
        {
            // Fallback conservador si no hay suficientes datos
            return CreateConservativeOutlook(countryName, principalUsd);
        }

        // 1. Normalizar indicadores (rangos específicos)
        var inflAvgNorm = NormalizeInflation(economicSummary.InflationStats.Average);
        var inflLastNorm = NormalizeInflation(economicSummary.InflationStats.Last ?? economicSummary.InflationStats.Average);
        var inflTrendNorm = NormalizeTrend(economicSummary.InflationStats.Trend, -5, 5);

        var unAvgNorm = NormalizeUnemployment(economicSummary.UnemploymentStats.Average);
        var unLastNorm = NormalizeUnemployment(economicSummary.UnemploymentStats.Last ?? economicSummary.UnemploymentStats.Average);
        var unTrendNorm = NormalizeTrend(economicSummary.UnemploymentStats.Trend, -3, 3);

        var gdpAvgNorm = NormalizeGdpGrowth(economicSummary.GdpGrowthStats.Average);
        var gdpLastNorm = NormalizeGdpGrowth(economicSummary.GdpGrowthStats.Last ?? economicSummary.GdpGrowthStats.Average);
        var gdpTrendNorm = NormalizeTrend(economicSummary.GdpGrowthStats.Trend, -6, 6);

        // 2. Calcular scores por indicador
        var inflScore = 0.60 * inflAvgNorm + 0.30 * inflLastNorm + 0.10 * inflTrendNorm;
        var unScore = 0.60 * unAvgNorm + 0.30 * unLastNorm + 0.10 * unTrendNorm;
        var gdpScore = 0.60 * gdpAvgNorm + 0.30 * gdpLastNorm + 0.10 * gdpTrendNorm;

        // 3. Normalizar volatilidad
        var inflStdNorm = NormalizeVolatility(economicSummary.InflationStats.StdDev, 0, 6);
        var unStdNorm = NormalizeVolatility(economicSummary.UnemploymentStats.StdDev, 0, 4);
        var gdpStdNorm = NormalizeVolatility(economicSummary.GdpGrowthStats.StdDev, 0, 6);

        // 4. Score global
        var globalGdpNorm = NormalizeGdpGrowth(economicSummary.GlobalGdpGrowth);
        var globalInflNorm = NormalizeInflation(economicSummary.GlobalInflation);
        var globalScore = 0.60 * globalGdpNorm + 0.40 * (1 - globalInflNorm);

        // 5. MacroScore base
        var macroScore01 = 0.30 * (1 - inflScore) +
                          0.25 * (1 - unScore) +
                          0.30 * gdpScore +
                          0.15 * globalScore;
        var macroScoreBase = macroScore01 * 100;

        // 6. Penalización por volatilidad
        var volPenalty01 = 0.10 * inflStdNorm + 0.08 * unStdNorm + 0.06 * gdpStdNorm;
        var macroScore = Math.Clamp(macroScoreBase - (volPenalty01 * 100), 0, 100);

        // 7. RiskScore final
        var riskScore = (int)Math.Round(100 - macroScore);
        var riskLevel = RiskRules.GetRiskLevel(riskScore);

        // 8. ROI base
        var roiBase = 0.12 - (riskScore / 100.0) * 0.18;
        roiBase = Math.Clamp(roiBase, -0.12, 0.18);

        // 9. Ajuste por momentum
        var momentum01 = 0.50 * gdpTrendNorm +
                        0.25 * (1 - inflTrendNorm) +
                        0.25 * (1 - unTrendNorm);
        var roiAdjust = (momentum01 - 0.5) * 0.04;
        roiBase = Math.Clamp(roiBase + roiAdjust, -0.12, 0.18);

        // 10. Spread dinámico
        var baseSpread = riskLevel switch
        {
            "BAJO" => 0.03,
            "MEDIO" => 0.05,
            "ALTO" => 0.08,
            _ => 0.05
        };
        var extraSpread = volPenalty01 * 0.05;
        var spread = baseSpread + extraSpread;

        var roiMin = Math.Clamp(roiBase - spread, -0.20, 0.25);
        var roiMax = Math.Clamp(roiBase + spread, -0.20, 0.25);

        // 11. Proyecciones compuestas (min, base, max)
        var y1 = CalculateProjections(principalUsd, roiMin, roiBase, roiMax, 1);
        var y3 = CalculateProjections(principalUsd, roiMin, roiBase, roiMax, 3);
        var y5 = CalculateProjections(principalUsd, roiMin, roiBase, roiMax, 5);

        // 12. Determinar decisión
        var decision = DetermineDecision(riskLevel, roiBase);

        return new InvestmentOutlookDto
        {
            Country = countryName,
            PrincipalUsd = principalUsd,
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            Decision = decision,
            RoiAnnual = new RoiAnnualDto
            {
                Min = roiMin,
                Base = roiBase,
                Max = roiMax
            },
            Projection = new OutlookProjectionDto
            {
                Y1 = y1,
                Y3 = y3,
                Y5 = y5
            }
        };
    }

    private static InvestmentOutlookDto CreateConservativeOutlook(string countryName, decimal principalUsd)
    {
        return new InvestmentOutlookDto
        {
            Country = countryName,
            PrincipalUsd = principalUsd,
            RiskScore = 70,
            RiskLevel = "ALTO",
            Decision = "AVOID",
            RoiAnnual = new RoiAnnualDto { Min = -0.12, Base = -0.06, Max = 0.0 },
            Projection = new OutlookProjectionDto
            {
                Y1 = new YearProjectionDto { FinalBase = principalUsd * 0.94m, Label = "DECREMENTO" },
                Y3 = new YearProjectionDto { FinalBase = principalUsd * 0.83m, Label = "DECREMENTO" },
                Y5 = new YearProjectionDto { FinalBase = principalUsd * 0.74m, Label = "DECREMENTO" }
            }
        };
    }

    private static double NormalizeInflation(double inflation)
    {
        // Rango: min=0, max=15
        return Math.Clamp(inflation / 15.0, 0, 1);
    }

    private static double NormalizeUnemployment(double unemployment)
    {
        // Rango: min=2, max=25
        return Math.Clamp((unemployment - 2) / 23.0, 0, 1);
    }

    private static double NormalizeGdpGrowth(double gdpGrowth)
    {
        // Rango: min=-10, max=10
        return Math.Clamp((gdpGrowth + 10) / 20.0, 0, 1);
    }

    private static double NormalizeTrend(double trend, double min, double max)
    {
        // Normalizar trend al rango especificado
        return Math.Clamp((trend - min) / (max - min), 0, 1);
    }

    private static double NormalizeVolatility(double stdDev, double min, double max)
    {
        // Normalizar desviación estándar
        return Math.Clamp((stdDev - min) / (max - min), 0, 1);
    }

    private static YearProjectionDto CalculateProjections(
        decimal principal,
        double roiMin,
        double roiBase,
        double roiMax,
        int years)
    {
        var finalBase = CalculateProjection(principal, roiBase, years);
        return new YearProjectionDto
        {
            FinalBase = finalBase,
            Label = ClassifyResult(finalBase, principal)
        };
    }

    private static decimal CalculateProjection(decimal principal, double roi, int years)
    {
        // Interés compuesto: finalValue = principal * (1 + roi)^years
        var multiplier = Math.Pow(1 + roi, years);
        return principal * (decimal)multiplier;
    }

    private static string ClassifyResult(decimal finalValue, decimal principal)
    {
        var changePercent = ((double)(finalValue - principal) / (double)principal) * 100;

        if (changePercent > 5)
        {
            return "CRECIMIENTO";
        }
        else if (changePercent >= -2)
        {
            return "MANTENIMIENTO";
        }
        else
        {
            return "DECREMENTO";
        }
    }

    private static string DetermineDecision(string riskLevel, double roiBase)
    {
        if (riskLevel == "ALTO")
        {
            return "AVOID";
        }
        else if (riskLevel == "MEDIO")
        {
            return "HOLD";
        }
        else if (riskLevel == "BAJO" && roiBase > 0)
        {
            return "INVEST";
        }
        else
        {
            return "HOLD";
        }
    }
}
