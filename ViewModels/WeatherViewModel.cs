using MAUI_Weather_App.Models;
using MAUI_Weather_App.Platforms;
using MAUI_Weather_App.Services;
using Microsoft.Maui.ApplicationModel;            // Permissions
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;             // Location
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MAUI_Weather_App.ViewModels;

public class WeatherViewModel : INotifyPropertyChanged
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;
    private CancellationTokenSource? _cts;

    public WeatherViewModel(
        IWeatherService weatherService,
        ILocationService locationService,
        IAudioService audioService)
    {
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

        FetchCommand = new Command(async () => await FetchAsync(), () => !IsBusy);
        UseLocationCommand = new Command(async () => await FetchByLocationAsync(), () => !IsBusy);
    }

    private string _cityInput = string.Empty;
    public string CityInput
    {
        get => _cityInput;
        set { _cityInput = value; OnPropertyChanged(); }
    }

    private string _temperature = "-";
    public string Temperature
    {
        get => _temperature;
        set { _temperature = value; OnPropertyChanged(); }
    }

    private string _description = "-";
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            ((Command)FetchCommand).ChangeCanExecute();
            ((Command)UseLocationCommand).ChangeCanExecute();
        }
    }

    public ICommand FetchCommand { get; }
    public ICommand UseLocationCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task FetchAsync()
    {
        if (IsBusy) return;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        IsBusy = true;
        Temperature = "...";
        Description = "Φορτώνει...";
        try
        {
            var city = (CityInput ?? string.Empty).Trim();
            var res = await _weatherService.GetByCityAsync(city, _cts.Token);
            if (res != null)
            {
                Temperature = $"{res.Main.Temp:F1} °C";
                Description = res.Weather.FirstOrDefault()?.Description ?? "-";
                CityInput = res.Name;
                _audioService.PlaySuccess();
            }
            else
            {
                Description = "Αδύνατη λήψη καιρικών στοιχείων.";
                Temperature = "-";
                _audioService.PlayFailure(); // fallback if audio failed (kept for parity) — see note below
            }
        }
        catch (OperationCanceledException)
        {
            Description = "Ακύρωση.";
            Temperature = "-";
        }
        catch (Exception)
        {
            Description = "Σφάλμα κατά την λήψη.";
            Temperature = "-";
            _audioService.PlayFailure(); // fallback if audio failed (kept for parity) — see note below
        }
        finally { IsBusy = false; }
    }

    private async Task FetchByLocationAsync()
    {
        if (IsBusy) return;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        IsBusy = true;
        Temperature = "...";
        Description = "Ζητάω άδεια τοποθεσίας...";

        // --- runtime permission request (important on Android/iOS) ---
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                Description = "Δεν δόθηκε άδεια τοποθεσίας.";
                Temperature = "-";
                _audioService.PlayFailure();
                IsBusy = false;
                return;
            }
        }
        catch (Exception)
        {
            // If Permissions API fails for some reason, fail gracefully
            Description = "Σφάλμα κατά το αίτημα αδειών.";
            Temperature = "-";
            _audioService.PlayFailure();
            IsBusy = false;
            return;
        }

        Description = "Βρίσκω τοποθεσία...";
        try
        {
            var loc = await _locationService.GetCurrentLocationAsync(_cts.Token);
            if (loc != null)
            {
                var res = await _weatherService.GetByCoordinatesAsync(loc.Latitude, loc.Longitude, _cts.Token);
                if (res != null)
                {
                    Temperature = $"{res.Main.Temp:F1} °C";
                    Description = res.Weather.FirstOrDefault()?.Description ?? "-";
                    CityInput = res.Name;
                    _audioService.PlaySuccess();
                }
                else
                {
                    Description = "Αδύνατη λήψη καιρικών στοιχείων για την τοποθεσία.";
                    Temperature = "-";
                    _audioService.PlayFailure(); // fallback if audio failed (kept for parity) — see note below
                }
            }
            else
            {
                Description = "Τοποθεσία μη διαθέσιμη ή άρνηση permissions.";
                Temperature = "-";
                _audioService.PlayFailure(); // fallback if audio failed (kept for parity) — see note below
            }
        }
        catch (OperationCanceledException)
        {
            Description = "Ακύρωση.";
            Temperature = "-";
        }
        catch (Exception)
        {
            Description = "Σφάλμα τοποθεσίας/καιρού.";
            Temperature = "-";
            _audioService.PlayFailure(); // fallback if audio failed (kept for parity) — see note below
        }
        finally { IsBusy = false; }
    }
}
