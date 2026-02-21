using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Services.Interfaces;

public interface IEconomicDataService
{
    Task<EconomicDataDto> GetEconomicDataAsync(string countryCode);
}
