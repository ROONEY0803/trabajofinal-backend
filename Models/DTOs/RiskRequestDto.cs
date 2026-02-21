using System.ComponentModel.DataAnnotations;

namespace CountryRiskAI.API.Models.DTOs;

public class RiskRequestDto
{
    [Required]
    public string CountryName { get; set; } = string.Empty;
}
