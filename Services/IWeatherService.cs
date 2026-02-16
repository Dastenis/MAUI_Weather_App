using MAUI_Weather_App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> GetByCityAsync(string city, CancellationToken ct = default);
        Task<WeatherResponse?> GetByCoordinatesAsync(double lat, double lon, CancellationToken ct = default);
    }
}