using System.Globalization;
using System.Windows.Data;

namespace BikeShare.Desktop.Converters;

public class StringToDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }
        return null;
    }
}