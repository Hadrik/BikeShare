using System.Collections.ObjectModel;
using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BikeShare.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Station> _stations = [
        new() {Id = 1, Name = "Station 1", Latitude = 40.7128, Longitude = -74.0060},
        new() {Id = 2, Name = "Station 2", Latitude = 34.0522, Longitude = -118.2437},
        new() {Id = 3, Name = "Station 3", Latitude = 51.5074, Longitude = -0.1278},
    ];
    
    [ObservableProperty]
    private ObservableCollection<Bike> _bikes = [
        new() {Id = 1, LastStatusChange = DateTime.Now, StationId = 1, Status = "Available"},
        new() {Id = 2, LastStatusChange = DateTime.Now, StationId = 2, Status = "Maintenance"},
        new() {Id = 3, LastStatusChange = DateTime.Now, StationId = 3, Status = "Available"},
    ];

    [ObservableProperty]
    private Dictionary<int, bool> _canRentFromStation = new();
    
    [ObservableProperty]
    private Rental? _activeRental;
    
    [ObservableProperty]
    private bool _hasActiveRental = false;

    public MainViewModel()
    {
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await LoadActiveRentalAsync();
        await LoadStationsAsync();
        await LoadBikesAsync();
    }

    [RelayCommand]
    private async Task LoadActiveRentalAsync()
    {
        ActiveRental = await ApiService.GetAsync<Rental>("rental/active");
        HasActiveRental = ActiveRental != null;
    }
    
    [RelayCommand]
    private async Task LoadStationsAsync()
    {
        var stations = await ApiService.GetAsync<List<Station>>("stations");
        if (stations != null)
        {
            Stations = new ObservableCollection<Station>(stations);

            foreach (var station in stations)
            {
                var bikes = await ApiService.GetAsync<int?>($"stations/{station.Id}/bikes");
                CanRentFromStation[station.Id] = !HasActiveRental && bikes != null && bikes > 0;
            }
            OnPropertyChanged(nameof(CanRentFromStation));
        }
    }
    
    [RelayCommand]
    private async Task LoadBikesAsync()
    {
        var bikes = await ApiService.GetAsync<List<Bike>>("stations/bikes");
        if (bikes != null)
        {
            Bikes = new ObservableCollection<Bike>(bikes);
        }
    }

    [RelayCommand]
    private async Task HandleRentalAsync(Station station)
    {
        if (HasActiveRental)
        {
            await ApiService.PostAsync("rental/end", new { stationId = station.Id });
            ActiveRental = null;
            HasActiveRental = false;
        }
        else
        {
            await ApiService.PostAsync("rental/start", new { stationId = station.Id });
            await LoadActiveRentalAsync();
        }
        
        await LoadStationsAsync();
        await LoadBikesAsync();
    }

    [RelayCommand]
    private async Task RefreshAll()
    {
        
    }

    [RelayCommand]
    private async Task EditStationAsync(Station station)
    {
        
    }
    
    [RelayCommand]
    private async Task DeleteStationAsync(Station station)
    {
        //TODO: add delete endpoint - delete station and move all bikes to maintenance
        // var result = await ApiService.DeleteAsync($"stations/{station.Id}");
        // if (result.IsSuccessStatusCode)
        // {
        //     Stations.Remove(station);
        // }
    }

    [RelayCommand]
    private async Task ToggleBikeStatusAsync(Bike bike)
    {
        var newStatus = bike.Status == "Available" ? "Maintenance" : "Available";
        await ApiService.PutAsync($"bikes/{bike.Id}/status", newStatus);
        await LoadBikesAsync();
    }
}