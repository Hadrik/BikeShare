using System.Collections.ObjectModel;
using System.Windows;
using BikeShare.Desktop.Models;
using BikeShare.Desktop.Views;
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
        new() {Id = 1, LastUpdated = DateTime.Now, StationId = 1, Status = "Available"},
        new() {Id = 2, LastUpdated = DateTime.Now, StationId = 2, Status = "Maintenance"},
        new() {Id = 3, LastUpdated = DateTime.Now, StationId = 3, Status = "Available"},
    ];
    
    [ObservableProperty]
    private Rental? _activeRental;
    
    [ObservableProperty]
    private bool _hasActiveRental = false;
    
    [ObservableProperty]
    private Station? _selectedStation;
    
    [ObservableProperty]
    private Bike? _selectedBike;

    public MainViewModel()
    {
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await ApiService.LoginAsync("app", "app");
        await LoadActiveRentalAsync();
        await LoadStationsAsync();
        await LoadBikesAsync();
    }

    [RelayCommand]
    private async Task LoadActiveRentalAsync()
    {
        ActiveRental = await ApiService.GetAsync<Rental>("rentals/active");
        HasActiveRental = ActiveRental != null;
    }

    public class RentalStats
    {
        public int Started { get; set; }
        public int Ended { get; set; }
    }
    
    [RelayCommand]
    private async Task LoadStationsAsync()
    {
        var stations = await ApiService.GetAsync<List<Station>>("stations");
        if (stations != null)
        {
            foreach (var station in stations)
            {
                var bikes = await ApiService.GetAsync<int?>($"stations/{station.Id}/bikes");
                station.CanRent = HasActiveRental || bikes != null && bikes > 0;
                
                var rentals = await ApiService.GetAsync<RentalStats>($"stats/station/{station.Id}/last-month");
                station.LastMonthRentalsStarted = rentals?.Started ?? 0;
                station.LastMonthRentalsEnded = rentals?.Ended ?? 0;
            }
            
            Stations = new ObservableCollection<Station>(stations);
        }
    }
    
    [RelayCommand]
    private async Task LoadBikesAsync()
    {
        var bikes = await ApiService.GetAsync<List<Bike>>("bikes");
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
            await ApiService.PostAsync($"rentals/end/{station.Id}", new{});
            ActiveRental = null;
            HasActiveRental = false;
        }
        else
        {
            await ApiService.PostAsync($"rentals/start/{station.Id}", new {});
            await LoadActiveRentalAsync();
        }
        
        await LoadStationsAsync();
        await LoadBikesAsync();
    }

    [RelayCommand]
    private async Task EditStationAsync(Station station)
    {
        var vm = new EditStationViewModel(station);
        var dialog = new EditStationView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        
        if (dialog.ShowDialog() == true)
        {
            var copy = vm.Copy;
            await ApiService.PutAsync($"stations/{station.Id}", new
            {
                copy.Name,
                copy.Latitude,
                copy.Longitude
            });

            await LoadStationsAsync();
        }
    }
    
    [RelayCommand]
    private async Task DeleteStationAsync()
    {
        if (SelectedStation == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete station '{SelectedStation.Name}'?", "Delete Station", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            await ApiService.DeleteAsync($"stations/{SelectedStation.Id}");
            await LoadStationsAsync();
        }
    }

    [RelayCommand]
    private async Task AddStationAsync()
    {
        var vm = new EditStationViewModel(new Station());
        var dialog = new EditStationView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        
        if (dialog.ShowDialog() == true)
        {
            var copy = vm.Copy;
            await ApiService.PostAsync("stations", new
            {
                copy.Name,
                copy.Latitude,
                copy.Longitude
            });

            await LoadStationsAsync();
        }
    }

    [RelayCommand]
    private async Task ChangeBikeStatusAsync(Bike bike)
    {
        var vm = new EditBikeStatusViewModel(bike);
        var dialog = new EditBikeStatusView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true)
        {
            var status = vm.SelectedStatus;
            var stationId = vm.SelectedStationId;
            
            await ApiService.PutAsync($"bikes/{bike.Id}/status", new
            {
                Status = status, 
                StationId = stationId
            });
            
            await LoadBikesAsync();
            await LoadStationsAsync();
        }
    }
    
    [RelayCommand]
    private async Task DeleteBikeAsync()
    {
        if (SelectedBike == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete bike '{SelectedBike.Id}'?", "Delete Bike", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            await ApiService.DeleteAsync($"bikes/{SelectedBike.Id}");
            await LoadBikesAsync();
        }
    }
    
    [RelayCommand]
    private async Task AddBikeAsync()
    {
        var vm = new CreateBikeViewModel();
        var dialog = new CreateBikeView()
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true)
        {
            var status = vm.SelectedStatus;
            var stationId = vm.SelectedStationId;
            
            await ApiService.PostAsync("bikes", new
            {
                Status = status, 
                StationId = stationId
            });
            
            await LoadBikesAsync();
            await LoadStationsAsync();
        }
    }

    [RelayCommand]
    private async Task ShowBikeStatsAsync(Bike bike)
    {
        var vm = new BikeHistoryViewModel(bike);
        var dialog = new BikeHistoryView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        dialog.ShowDialog();
    }
}