using CountryRiskAI.API.Clients;
using CountryRiskAI.API.Clients.Interfaces;
using CountryRiskAI.API.Middleware;
using CountryRiskAI.API.Services;
using CountryRiskAI.API.Services.Interfaces;
using DotNetEnv;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var geminiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (!string.IsNullOrEmpty(geminiKey))
{
    builder.Configuration["ExternalApis:Gemini:ApiKey"] = geminiKey;
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// HTTP Clients
builder.Services.AddHttpClient<IRestCountriesClient, RestCountriesClient>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:RestCountries:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl ?? "https://restcountries.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<WorldBankClient>(client =>
{
    client.BaseAddress = new Uri("https://api.worldbank.org");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHttpClient<IGeminiClient, GeminiClient>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:Gemini:BaseUrl"];
    var timeoutSeconds = builder.Configuration.GetValue<int>("ExternalApis:Gemini:TimeoutSeconds", 30);

    client.BaseAddress = new Uri(baseUrl ?? "https://generativelanguage.googleapis.com");
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

// Services
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IEconomicDataService, EconomicDataService>();
builder.Services.AddScoped<IGlobalEconomicService, GlobalEconomicService>();
builder.Services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();
builder.Services.AddScoped<IInvestmentAnalysisService, InvestmentAnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection solo si hay HTTPS configurado
var httpsPort = Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT");
if (!string.IsNullOrEmpty(httpsPort))
{
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Endpoint raíz (solo si no hay archivos estáticos que responder)
app.MapGet("/api", () =>
{
    if (app.Environment.IsDevelopment())
    {
        return Results.Redirect("/swagger");
    }

    return Results.Json(new
    {
        name = "CountryRisk AI API",
        version = "1.0",
        endpoints = new
        {
            country = "/api/country/{name}",
            analysis = "/api/analysis/risk"
        }
    });
});

app.Run();
