using System.Windows;
using System.Windows.Media;

namespace ChronoPlay.Controls;

public static class TextBoxIcon
{
    public static readonly DependencyProperty GeometryProperty =
        DependencyProperty.RegisterAttached(
            "Geometry",
            typeof(Geometry),
            typeof(TextBoxIcon),
            new FrameworkPropertyMetadata(null));

    public static Geometry? GetGeometry(DependencyObject element) =>
        (Geometry?)element.GetValue(GeometryProperty);

    public static void SetGeometry(DependencyObject element, Geometry? value) =>
        element.SetValue(GeometryProperty, value);
}
