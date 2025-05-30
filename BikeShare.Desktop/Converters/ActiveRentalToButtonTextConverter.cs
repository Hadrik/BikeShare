﻿using System.Globalization;
using System.Windows.Data;

namespace BikeShare.Desktop.Converters;

public class ActiveRentalToButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value ? "End Rental" : "Start Rental";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}