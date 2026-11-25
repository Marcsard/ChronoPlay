using System.Globalization;
using System.Windows.Data;

namespace ChronoPlay.Controls;

public sealed class DateTimeDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime || dateTime == DateTime.MinValue)
        {
            return "—";
        }

        string format = parameter as string ?? "g";
        return dateTime.ToString(format, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
