using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MAUI_Weather_App.Models
{

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

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Coord
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; }


        [JsonPropertyName("lat")]
        public double Lat { get; set; }
    }

    public class WeatherItem
    {
        [JsonPropertyName("main")] 
        public string Main { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("icon")] 
        public string Icon { get; set; } = string.Empty;
    }

    public class Main
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("humidity")] 
        public int Humidity { get; set; }

        [JsonPropertyName("pressure")] 
        public double Pressure { get; set; }

    }

    public class Wind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}