using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChronoPlay.Controls;

public sealed class UserAvatarValueConverter : IValueConverter
{
    private static readonly Color[] AvatarColors =
    {
        Color.FromRgb(79, 70, 229),
        Color.FromRgb(8, 145, 178),
        Color.FromRgb(124, 58, 237),
        Color.FromRgb(37, 99, 235),
        Color.FromRgb(5, 150, 105)
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string name = value as string ?? string.Empty;
        string mode = parameter as string ?? string.Empty;

        if (mode == "Initial")
        {
            string trimmedName = name.Trim();
            return trimmedName.Length == 0
                ? string.Empty
                : char.ToUpperInvariant(trimmedName[0]).ToString();
        }

        int hash = 0;
        foreach (char character in name.Trim())
        {
            hash = unchecked(hash * 31 + char.ToUpperInvariant(character));
        }

        int colorIndex = (int)((uint)hash % (uint)AvatarColors.Length);
        SolidColorBrush brush = new(AvatarColors[colorIndex]);
        brush.Freeze();
        return brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
