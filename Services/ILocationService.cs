using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    public interface ILocationService
    {
        Task<Location?> GetCurrentLocationAsync(CancellationToken ct = default);
    }
}
