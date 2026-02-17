using MAUI_Weather_App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    /// <summary>
    /// Abstraction for retrieving weather data.
    /// Implementations are responsible for communicating with external providers
    /// and returning a mapped WeatherResponse DTO.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Retrieves weather information using a city name.
        /// Returns null when the request fails or data is unavailable.
        /// </summary>
        Task<WeatherResponse?> GetByCityAsync(string city, CancellationToken ct = default);

        /// <summary>
        /// Retrieves weather information using geographic coordinates.
        /// Returns null when the request fails or data is unavailable.
        /// </summary>
        Task<WeatherResponse?> GetByCoordinatesAsync(double lat, double lon, CancellationToken ct = default);
    }
}
