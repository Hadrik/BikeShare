using System.Collections.ObjectModel;
using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BikeShare.Desktop.ViewModels;

public partial class StationViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Station> _stations = [];

    public StationViewModel()
    {
        LoadStations();
    }
    
    private void LoadStations()
    {
        // Simulate loading stations from a data source
        Stations = new ObservableCollection<Station>
        {
            new Station { Id = 1, Name = "Station 1", Latitude = 40.7128, Longitude = -74.0060 },
            new Station { Id = 2, Name = "Station 2", Latitude = 34.0522, Longitude = -118.2437 },
            new Station { Id = 3, Name = "Station 3", Latitude = 51.5074, Longitude = -0.1278 }
        };
    }

    [RelayCommand]
    private async Task LoadStationsFromApiAsync()
    {
        var stations = await ApiService.GetAsync<List<Station>>("stations");
        if (stations != null)
        {
            Stations = new ObservableCollection<Station>(stations);
        }
    }
}