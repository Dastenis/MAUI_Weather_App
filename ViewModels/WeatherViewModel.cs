using MAUI_Weather_App.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MAUI_Weather_App.ViewModels;

/// <summary>
/// Handles weather retrieval logic and exposes bindable UI state.
/// Coordinates API calls, permissions and audio feedback.
/// </summary>
public class WeatherViewModel : INotifyPropertyChanged
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;

    // Cancels any in-flight request when a new one starts
    private CancellationTokenSource? _cts;

    public WeatherViewModel(
        IWeatherService weatherService,
        ILocationService locationService,
        IAudioService audioService)
    {
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

        // Commands disabled while a request is running
        FetchCommand = new Command(async () => await FetchAsync(), () => !IsBusy);
        UseLocationCommand = new Command(async () => await FetchByLocationAsync(), () => !IsBusy);
    }

    // User input city
    private string _cityInput = string.Empty;
    public string CityInput
    {
        get => _cityInput;
        set { _cityInput = value; OnPropertyChanged(); }
    }

    // Display temperature text
    private string _temperature = "-";
    public string Temperature
    {
        get => _temperature;
        set { _temperature = value; OnPropertyChanged(); }
    }

    // Display weather description text
    private string _description = "-";
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    // Indicates active network/location request
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;

            _isBusy = value;
            OnPropertyChanged();

            // Update command availability
            ((Command)FetchCommand).ChangeCanExecute();
            ((Command)UseLocationCommand).ChangeCanExecute();
        }
    }

    public ICommand FetchCommand { get; }
    public ICommand UseLocationCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>
    /// Retrieves weather data using user provided city name.
    /// Cancels any previous request and updates UI state.
    /// </summary>
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
                Temperature = "-";
                Description = "Αδύνατη λήψη καιρικών στοιχείων.";

                _audioService.PlayFailure();
            }
        }
        catch (OperationCanceledException)
        {
            Temperature = "-";
            Description = "Ακύρωση.";
        }
        catch (Exception)
        {
            Temperature = "-";
            Description = "Σφάλμα κατά την λήψη.";

            _audioService.PlayFailure();
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Retrieves weather data using device geolocation.
    /// Handles runtime permission flow and failure states.
    /// </summary>
    private async Task FetchByLocationAsync()
    {
        if (IsBusy) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        IsBusy = true;
        Temperature = "...";
        Description = "Ζητάω άδεια τοποθεσίας...";

        // Request runtime location permission
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                Temperature = "-";
                Description = "Δεν δόθηκε άδεια τοποθεσίας.";

                _audioService.PlayFailure();
                IsBusy = false;
                return;
            }
        }
        catch (Exception)
        {
            Temperature = "-";
            Description = "Σφάλμα κατά το αίτημα αδειών.";

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
                    Temperature = "-";
                    Description = "Αδύνατη λήψη καιρικών στοιχείων για την τοποθεσία.";

                    _audioService.PlayFailure();
                }
            }
            else
            {
                Temperature = "-";
                Description = "Τοποθεσία μη διαθέσιμη ή άρνηση permissions.";

                _audioService.PlayFailure();
            }
        }
        catch (OperationCanceledException)
        {
            Temperature = "-";
            Description = "Ακύρωση.";
        }
        catch (Exception)
        {
            Temperature = "-";
            Description = "Σφάλμα τοποθεσίας/καιρού.";

            _audioService.PlayFailure();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
