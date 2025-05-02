﻿using System.Globalization;
using System.Windows.Data;
using BikeShare.Desktop.ViewModels;

namespace BikeShare.Desktop.Converters;

public class StationIdToEnabledConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is int stationId && values[1] is MainViewModel viewModel)
        {
            return viewModel.CanRentFromStation.TryGetValue(stationId, out var canRent) && canRent;
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}