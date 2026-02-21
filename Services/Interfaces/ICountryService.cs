using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Services.Interfaces;

public interface ICountryService
{
    Task<CountryDto?> GetCountryAsync(string countryName);
}
