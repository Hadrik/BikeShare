﻿using System.Windows;
using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BikeShare.Desktop.ViewModels;

public partial class EditStationViewModel : ObservableObject
{
    [ObservableProperty]
    private Station _copy;

    public EditStationViewModel(Station station)
    {
        Copy = new Station
        {
            Id = station.Id,
            Name = station.Name,
            Latitude = station.Latitude,
            Longitude = station.Longitude
        };
    }

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