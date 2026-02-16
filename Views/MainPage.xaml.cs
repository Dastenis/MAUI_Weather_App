using MAUI_Weather_App.ViewModels;
using Plugin.Maui.Audio;
using System.Diagnostics;

namespace MAUI_Weather_App.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WeatherViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void  Test_Audio(object sender, EventArgs e)
        {
            try
            {
                var audio = AudioManager.Current;
                // δοκίμασε με όνομα πρώτα
                var player = audio.CreatePlayer("success.wav");
                if (player == null)
                {
                    // fallback: stream
                    using var s = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/success.wav");
                    player = audio.CreatePlayer(s);
                }
                if (player != null) player.Play();
                else Debug.WriteLine("player null — file not found in package");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("audio error: " + ex);
            }
        }
    }
}
