using MAUI_Weather_App.Services;
using MAUI_Weather_App.ViewModels;
using MAUI_Weather_App.Views;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

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

            // other services...
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>();

            // Register audio manager (Plugin.Maui.Audio)
            builder.Services.AddSingleton(AudioManager.Current);

            // register audio manager interface explicitly (optionally)
            builder.Services.AddSingleton<IAudioManager>(_ => AudioManager.Current);

            // remaining registrations...
            builder.Services.AddSingleton<ILocationService, GeolocationService>();
            builder.Services.AddSingleton<IAudioService, /*optional wrapper*/ MAUI_Weather_App.Services.Audio.MauiAudioService>();
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<Views.MainPage>();

            return builder.Build();
        }
    }
}