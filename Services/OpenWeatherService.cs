using System.Text.Json;
using MAUI_Weather_App.Models;
using Microsoft.Extensions.Configuration;

namespace MAUI_Weather_App.Services;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    // inject IConfiguration
    public OpenWeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

        // Προτεραιότητα: IConfiguration (user-secrets / appsettings) -> περιβάλλον -> fallback placeholder
        _apiKey = "4fd8cf20fbe253b29756884d5f67f6b7";
    }

    public async Task<WeatherResponse?> GetByCityAsync(string city, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city)) return null;
        var url = $"weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=el";
        return await Fetch(url, ct);
    }

    public async Task<WeatherResponse?> GetByCoordinatesAsync(double lat, double lon, CancellationToken ct = default)
    {
        var url = $"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric&lang=el";
        return await Fetch(url, ct);
    }

    private async Task<WeatherResponse?> Fetch(string relativeUrl, CancellationToken ct)
    {
        try
        {
            using var response = await _http.GetAsync(relativeUrl, ct);
            if (!response.IsSuccessStatusCode) return null;
            var stream = await response.Content.ReadAsStreamAsync(ct);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = await JsonSerializer.DeserializeAsync<WeatherResponse>(stream, opts, ct);
            return data;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception)
        {
            return null;
        }
    }
}
