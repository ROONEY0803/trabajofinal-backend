namespace CountryRiskAI.API.Helpers;

public static class RiskParser
{
    public static (int Score, string Recommendation) ParseAiResponse(string? aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return (50, "No se pudo obtener análisis de riesgo.");
        }

        // Normalizar: remover espacios extra y convertir a mayúsculas para búsqueda
        var normalized = aiResponse.Trim();
        int score = 50;
        string recommendation = "Análisis no disponible.";

        // Buscar SCORE: en cualquier formato (case insensitive, con o sin espacios)
        var scoreMatch = System.Text.RegularExpressions.Regex.Match(
            normalized,
            @"SCORE\s*:\s*(\d+)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (scoreMatch.Success && scoreMatch.Groups.Count > 1)
        {
            if (int.TryParse(scoreMatch.Groups[1].Value, out var parsedScore))
            {
                score = Math.Clamp(parsedScore, 0, 100);
            }
        }
        else
        {
            // Fallback: buscar cualquier número entre 0-100 en el texto
            var numberMatches = System.Text.RegularExpressions.Regex.Matches(
                normalized,
                @"\b([0-9]|[1-9][0-9]|100)\b");

            foreach (System.Text.RegularExpressions.Match match in numberMatches)
            {
                if (int.TryParse(match.Value, out var num) && num >= 0 && num <= 100)
                {
                    score = num;
                    break;
                }
            }
        }

        // Buscar RECOMENDACIÓN o RECOMMENDATION
        var recMatch = System.Text.RegularExpressions.Regex.Match(
            normalized,
            @"(?:RECOMENDACIÓN|RECOMMENDATION)\s*:\s*(.+?)(?:\n|$)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
            System.Text.RegularExpressions.RegexOptions.Singleline);

        if (recMatch.Success && recMatch.Groups.Count > 1)
        {
            recommendation = CleanText(recMatch.Groups[1].Value);
        }
        else
        {
            // Fallback: tomar la última línea si no hay formato claro
            var lines = normalized.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 1)
            {
                recommendation = CleanText(lines.Last());
            }
        }

        return (score, recommendation);
    }

    private static string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        // Reemplazar saltos de línea con espacios
        var cleaned = text
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");

        // Eliminar espacios múltiples
        while (cleaned.Contains("  "))
        {
            cleaned = cleaned.Replace("  ", " ");
        }

        return cleaned.Trim();
    }
}
