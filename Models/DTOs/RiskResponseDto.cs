namespace CountryRiskAI.API.Models.DTOs;

public class RiskResponseDto
{
    public string Country { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}
