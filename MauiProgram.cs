using MAUI_Weather_App.Services;
using MAUI_Weather_App.ViewModels;
using MAUI_Weather_App.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // <- για AddHttpClient
using Plugin.Maui.Audio; // AudioManager

namespace MAUI_Weather_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { /* ... */ });

            // HttpClientFactory (requires Microsoft.Extensions.Http package)
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            // Register audio manager (Plugin.Maui.Audio) as singleton
            builder.Services.AddSingleton<IAudioManager>(_ => AudioManager.Current);
            builder.Services.AddSingleton<IAudioService, MAUI_Weather_App.Services.Audio.MauiAudioService>();

            // Register a platform-agnostic IAudioService wrapper that uses AudioManager
            builder.Services.AddSingleton<IAudioService, Services.Audio.MauiAudioService>();

            // Other services
            builder.Services.AddSingleton<ILocationService, GeolocationService>();

            // ViewModels & Views
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
