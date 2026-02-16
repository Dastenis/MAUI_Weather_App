using System.Text.Json;
using MAUI_Weather_App.Models;

namespace MAUI_Weather_App.Services;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public OpenWeatherService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? "REPLACE_WITH_YOUR_API_KEY";
    }

    public async Task<WeatherResponse?> GetByCityAsync(string city, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city)) return null;
        var url = $"weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=el";
        return await FetchAsync(url, ct);
    }

    public async Task<WeatherResponse?> GetByCoordinatesAsync(double lat, double lon, CancellationToken ct = default)
    {
        var url = $"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric&lang=el";
        return await FetchAsync(url, ct);
    }

    private async Task<WeatherResponse?> FetchAsync(string relativeUrl, CancellationToken ct)
    {
        try
        {
            using var res = await _http.GetAsync(relativeUrl, ct);
            if (!res.IsSuccessStatusCode) return null;
            await using var stream = await res.Content.ReadAsStreamAsync(ct);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = await JsonSerializer.DeserializeAsync<WeatherResponse>(stream, opts, ct);
            return data;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception)
        {
            // log if desired
            return null;
        }
    }
}
