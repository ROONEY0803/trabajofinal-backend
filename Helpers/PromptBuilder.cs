using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Helpers;

public static class PromptBuilder
{
    public static string BuildRiskAnalysisPrompt(
        CountryDto country,
        EconomicSummaryDto economicSummary)
    {
        return $@"Evalúa el riesgo de inversión para {country.Name} basándote en estos datos procesados:

DATOS GENERALES:
- Población: {country.Population:N0} habitantes
- Región: {country.Region}
- Moneda: {country.Currency}
- Área: {country.Area:N2} km²

INDICADORES ECONÓMICOS (últimos 10 años):
- Inflación promedio: {economicSummary.AverageInflation:F2}% (tendencia: {economicSummary.InflationTrend})
- Crecimiento PIB promedio: {economicSummary.AverageGdpGrowth:F2}% (tendencia: {economicSummary.GdpGrowthTrend})
- Última tasa de desempleo: {(economicSummary.LastUnemploymentRate.HasValue ? $"{economicSummary.LastUnemploymentRate:F2}%" : "No disponible")} (tendencia: {economicSummary.UnemploymentTrend})

CONTEXTO GLOBAL:
- Crecimiento PIB mundial: {economicSummary.GlobalGdpGrowth:F2}%
- Inflación global: {economicSummary.GlobalInflation:F2}%
- Comparación: {economicSummary.ComparisonWithGlobal}

IMPORTANTE: Responde EXACTAMENTE en este formato (sin texto adicional):

SCORE: [número entre 0 y 100]
RECOMENDACIÓN: [texto breve en español]

Ejemplo:
SCORE: 65
RECOMENDACIÓN: Riesgo moderado, considerar inversión con precaución.";
    }

    public static string BuildInvestmentInterpretationPrompt(InvestmentOutlookDto outlook)
    {
        var roiBase = outlook.RoiAnnual.Base * 100;
        var roiMin = outlook.RoiAnnual.Min * 100;
        var roiMax = outlook.RoiAnnual.Max * 100;

        return $@"Rol: Resume para un inversionista el análisis de inversión en {outlook.Country}. Usa tono profesional, directo y neutro.

DATOS PROCESADOS (backend):
- Inversión: ${outlook.PrincipalUsd:N0} USD
- Riesgo: {outlook.RiskLevel} (Score: {outlook.RiskScore}/100)
- Decisión: {outlook.Decision}
- ROI anual: {roiBase:F2}% (rango {roiMin:F2}%-{roiMax:F2}%)
- 1 año: ${outlook.Projection.Y1.FinalBase:N0} ({outlook.Projection.Y1.Label}) | 3 años: ${outlook.Projection.Y3.FinalBase:N0} | 5 años: ${outlook.Projection.Y5.FinalBase:N0} ({outlook.Projection.Y5.Label})

Escribe 2-3 oraciones en español que:
1) Resuman el nivel de riesgo y la decisión
2) Mencionen la proyección de retorno
3) Terminen con: estimaciones basadas en indicadores históricos, no constituye asesoría financiera.

Ejemplo de estilo: Riesgo bajo con score 38/100. Decisión: invertir. ROI esperado ~12% anual; proyección a 5 años: $440k. Estimaciones basadas en indicadores históricos, no constituye asesoría financiera.";
    }
}
