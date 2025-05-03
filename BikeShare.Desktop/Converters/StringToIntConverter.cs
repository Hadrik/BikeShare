using System.Globalization;
using System.Windows.Data;

namespace BikeShare.Desktop.Converters;

public class StringToIntConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            return i.ToString();
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            if (int.TryParse(s, out var result))
            {
                return result;
            }
        }
        return null;
    }
}