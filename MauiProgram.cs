using MAUI_Weather_App.ViewModels;
using MAUI_Weather_App.Views;
using MAUI_Weather_App.Services;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.Audio;
using System;

namespace MAUI_Weather_App
{
    /// <summary>
    /// Application composition root.
    /// Configures DI, typed HttpClient and platform-agnostic services.
    /// </summary>
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { /* register fonts here */ });

            // Typed HttpClient for OpenWeather (configured in DI)
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            // Plugin.Maui.Audio manager and single cross-platform IAudioService
            builder.Services.AddSingleton<IAudioManager>(_ => AudioManager.Current);
            builder.Services.AddSingleton<IAudioService, Services.Audio.MauiAudioService>();

            // Cross-platform location service
            builder.Services.AddSingleton<ILocationService, GeolocationService>();

            // MVVM registrations
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
