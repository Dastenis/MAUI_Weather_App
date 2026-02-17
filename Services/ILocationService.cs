using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    /// <summary>
    /// Provides access to the device's current geographic location.
    /// Implementations handle platform-specific APIs and permission requirements.
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Attempts to obtain the current device location.
        /// Returns null when location is unavailable, denied, or cannot be resolved.
        /// </summary>
        Task<Location?> GetCurrentLocationAsync(CancellationToken ct = default);
    }
}
