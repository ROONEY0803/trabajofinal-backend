namespace CountryRiskAI.API.Clients.Interfaces;

public interface IGeminiClient
{
    Task<string?> GenerateTextAsync(string prompt);
}
