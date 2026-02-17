using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    /// <summary>
    /// Default implementation of ILocationService using MAUI Essentials geolocation APIs.
    /// Handles runtime permission flow and gracefully degrades on unsupported devices.
    /// </summary>
    public class GeolocationService : ILocationService
    {
        /// <summary>
        /// Attempts to retrieve the device's current location.
        /// Returns null when permissions are denied, unavailable, or any platform failure occurs.
        /// </summary>
        public async Task<Location?> GetCurrentLocationAsync(CancellationToken ct = default)
        {
            try
            {
                // Ensure location permission is granted before accessing sensor
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                        return null;
                }

                // Balanced accuracy to reduce battery usage while remaining usable for city-level weather
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                // Query platform location provider
                var location = await Geolocation.Default.GetLocationAsync(request, ct);

                return location;
            }
            catch (FeatureNotSupportedException)
            {
                // Device does not support location services
                return null;
            }
            catch (PermissionException)
            {
                // Permission revoked during request
                return null;
            }
            catch (Exception)
            {
                // Any other platform failure (timeouts, provider errors, etc.)
                return null;
            }
        }
    }
}
