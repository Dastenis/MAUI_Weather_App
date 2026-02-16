using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MAUI_Weather_App.Models;
using MAUI_Weather_App.Services;
using Microsoft.Maui.Controls;

namespace MAUI_Weather_App.ViewModels;

public class WeatherViewModel : INotifyPropertyChanged
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;
    private CancellationTokenSource? _cts;

    public WeatherViewModel(IWeatherService weatherService, ILocationService locationService, IAudioService audioService)
    {
        _weatherService = weatherService;
        _locationService = locationService;
        _audio_service = audioService; // will be assigned to consistent name below

        FetchCommand = new Command(async () => await FetchAsync(), () => !IsBusy);
        UseLocationCommand = new Command(async () => await FetchByLocationAsync(), () => !IsBusy);
    }

    private string _cityInput = string.Empty;
    public string CityInput { get => _cityInput; set { _cityInput = value; OnPropertyChanged(); } }

    private string _temperature = "-";
    public string Temperature { get => _temperature; set { _temperature = value; OnPropertyChanged(); } }

    private string _description = "-";
    public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

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
            var res = await _weatherService.GetByCityAsync(CityInput.Trim(), _cts.Token);
            if (res != null)
            {
                Temperature = $"{res.Main.Temp:F1} °C";
                Description = res.Weather.FirstOrDefault()?.Description ?? "-";
                CityInput = res.Name;
                _audio_service.PlaySuccess();
            }
            else
            {
                Description = "Αδύνατη λήψη καιρικών στοιχείων.";
                Temperature = "-";
                _audio_service.PlayFailure();
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
            _audio_service.PlayFailure();
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
        Description = "Προσπαθώ να βρω τοποθεσία...";
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
                    _audio_service.PlaySuccess();
                }
                else
                {
                    Description = "Αδύνατη λήψη καιρικών στοιχείων για την τοποθεσία.";
                    Temperature = "-";
                    _audio_service.PlayFailure();
                }
            }
            else
            {
                Description = "Τοποθεσία μη διαθέσιμη ή άρνηση permissions.";
                Temperature = "-";
                _audio_service.PlayFailure();
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
            _audio_service.PlayFailure();
        }
        finally { IsBusy = false; }
    }

    // internal consistent alias for audio service used above
    private readonly IAudioService _audio_service;
}
