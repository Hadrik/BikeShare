using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BikeShare.Desktop.ViewModels;

public partial class EditStationViewModel : ObservableObject
{
    [ObservableProperty]
    private Station _tempStation;

    public EditStationViewModel(Station station)
    {
        TempStation = new Station
        {
            Id = station.Id,
            Name = station.Name,
            Latitude = station.Latitude,
            Longitude = station.Longitude
        };
    }

    // [RelayCommand]
    // private void Save(Window window)
    // {
    //     
    // }
}