using MAUI_Weather_App.ViewModels;

namespace MAUI_Weather_App.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WeatherViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
