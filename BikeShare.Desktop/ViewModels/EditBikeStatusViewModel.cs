using System.Windows;
using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BikeShare.Desktop.ViewModels;

public partial class EditBikeStatusViewModel(Bike bike) : ObservableObject
{
    public Bike Original { get; } = bike;
    public Bike Edit { get; set; } = new Bike()
    {
        Id = bike.Id,
        LastUpdated = bike.LastUpdated,
        StationId = bike.StationId,
        Status = bike.Status
    };
    
    public string[] Statuses { get; } =
    [
        "Available",
        "Maintenance"
    ];

    [ObservableProperty]
    private string _selectedStatus = "";
    
    [ObservableProperty]
    private int? _selectedStationId;

    [RelayCommand]
    private void Cancel()
    {
        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }

    [RelayCommand]
    private void Submit()
    {
        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }
}