using System.Text.Json;
using MAUI_Weather_App.Models;
using Microsoft.Extensions.Configuration;

namespace MAUI_Weather_App.Services;

/// <summary>
/// Lightweight OpenWeather API client.
/// - Uses typed HttpClient injected by DI.
/// - Exposes simple methods for city and coordinate lookups.
/// - Returns domain DTO (WeatherResponse) or null on non-success/failure.
/// </summary>
public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    /// <summary>
    /// Constructor — HttpClient is provided by HttpClientFactory (typed client).
    /// IConfiguration is available for retrieving API keys from secure sources
    /// (user-secrets, environment, appsettings). Avoid committing secrets to source.
    /// </summary>
    public OpenWeatherService(HttpClient http, IConfiguration config)
    {
        // HttpClient instance from DI. Keep BaseAddress consistent with service registration.
        _http = http;
        _http.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

        // Configuration priority (recommended pattern):
        // 1) IConfiguration (user-secrets / appsettings)
        // 2) Environment variables
        // 3) Fallback placeholder or throw in production code
        //
        // NOTE: The key below is currently hardcoded — replace with config lookup before shipping.
        _apiKey = "4fd8cf20fbe253b29756884d5f67f6b7";
    }

    /// <summary>
    /// Fetch weather by city name.
    /// Returns null for empty input or non-successful responses.
    /// </summary>
    public async Task<WeatherResponse?> GetByCityAsync(string city, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city)) return null;

        // Escape user input to avoid malformed requests
        var url = $"weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=el";
        return await Fetch(url, ct);
    }

    /// <summary>
    /// Fetch weather by geographic coordinates.
    /// </summary>
    public async Task<WeatherResponse?> GetByCoordinatesAsync(double lat, double lon, CancellationToken ct = default)
    {
        var url = $"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric&lang=el";
        return await Fetch(url, ct);
    }

    /// <summary>
    /// Core HTTP fetch logic:
    /// - Calls the relative endpoint using the injected HttpClient.
    /// - Returns deserialized WeatherResponse on HTTP 2xx.
    /// - Propagates OperationCanceledException to support cooperative cancellation.
    /// - Converts other failures to null to keep callers simple (caller decides UX).
    /// </summary>
    private async Task<WeatherResponse?> Fetch(string relativeUrl, CancellationToken ct)
    {
        try
        {
            using var response = await _http.GetAsync(relativeUrl, ct);

            // Treat non-success responses as "no data" (null). Caller shows appropriate UX.
            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync(ct);

            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize directly from stream to minimize allocations
            var data = await JsonSerializer.DeserializeAsync<WeatherResponse>(stream, opts, ct);
            return data;
        }
        catch (OperationCanceledException)
        {
            // Preserve cancellation semantics for upstream handling
            throw;
        }
        catch (Exception)
        {
            // Swallow other exceptions and return null to simplify caller error handling.
            // Consider logging the exception in production code (ILogger injection).
            return null;
        }
    }
}
