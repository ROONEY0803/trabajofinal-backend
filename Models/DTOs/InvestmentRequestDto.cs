using System.ComponentModel.DataAnnotations;

namespace CountryRiskAI.API.Models.DTOs;

public class InvestmentRequestDto
{
    [Required]
    public string CountryName { get; set; } = string.Empty;

    [Range(1000, 10000000)]
    public decimal PrincipalUsd { get; set; } = 250000;
}
