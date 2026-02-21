using CountryRiskAI.API.Models.External;

namespace CountryRiskAI.API.Clients.Interfaces;

public interface IRestCountriesClient
{
    Task<RestCountriesResponse?> GetCountryAsync(string countryName);
}
