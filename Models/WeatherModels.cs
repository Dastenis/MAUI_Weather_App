using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MAUI_Weather_App.Models
{
    /// <summary>
    /// Root DTO mapped from OpenWeather API response.
    /// Contains only fields required by the application UI.
    /// </summary>
    public class WeatherResponse
    {
        [JsonPropertyName("coord")]
        public Coord Coord { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<WeatherItem> Weather { get; set; } = new();

        [JsonPropertyName("main")]
        public Main Main { get; set; } = new();

        [JsonPropertyName("wind")]
        public Wind Wind { get; set; } = new();

        // City name returned by API (normalized spelling)
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Geographic coordinates of the location.
    /// </summary>
    public class Coord
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }
    }

    /// <summary>
    /// Weather condition descriptor (first item typically used).
    /// </summary>
    public class WeatherItem
    {
        // Short condition label (e.g. Rain, Clouds)
        [JsonPropertyName("main")]
        public string Main { get; set; } = string.Empty;

        // Localized human-readable description
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // Icon identifier used for UI rendering
        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }

    /// <summary>
    /// Main atmospheric measurements.
    /// </summary>
    public class Main
    {
        // Temperature in Celsius (metric units requested)
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        // Relative humidity percentage
        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        // Atmospheric pressure (hPa)
        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }
    }

    /// <summary>
    /// Wind measurements.
    /// </summary>
    public class Wind
    {
        // Wind speed (m/s in metric mode)
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}
