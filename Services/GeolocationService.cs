using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    public class GeolocationService : ILocationService
    {
        public async Task<Location?> GetCurrentLocationAsync(CancellationToken ct = default)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted) return null;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request, ct);
                return location;
            }
            catch (FeatureNotSupportedException) { return null; }
            catch (PermissionException) { return null; }
            catch (Exception) { return null; }
        }
    }
}
