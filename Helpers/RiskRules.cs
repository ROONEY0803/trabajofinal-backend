namespace CountryRiskAI.API.Helpers;

public static class RiskRules
{
    public static string GetRiskLevel(int score)
    {
        if (score < 40)
        {
            return "BAJO";
        }

        if (score < 70)
        {
            return "MEDIO";
        }

        return "ALTO";
    }
}
