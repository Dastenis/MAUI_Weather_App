using MAUI_Weather_App.ViewModels;
using Plugin.Maui.Audio;
using System.Diagnostics;

namespace MAUI_Weather_App.Views
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Main application page.
        /// ViewModel is injected via dependency injection.
        /// </summary>
        public MainPage(WeatherViewModel vm)
        {
            InitializeComponent();

            // Assign ViewModel for data binding
            BindingContext = vm;
        }

    }
}
