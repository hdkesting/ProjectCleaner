namespace SolutionCleaner.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

internal class CountToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var count = value as int?;
        if (count.HasValue)
        {
            return count.Value.ToString("N0");
        }

        return "…";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
