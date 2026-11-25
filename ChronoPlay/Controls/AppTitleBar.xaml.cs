using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

namespace ChronoPlay.Controls;

public partial class AppTitleBar : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(AppTitleBar),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconGeometryProperty =
        DependencyProperty.Register(
            nameof(IconGeometry),
            typeof(Geometry),
            typeof(AppTitleBar));

    public static readonly DependencyProperty IconImageSourceProperty =
        DependencyProperty.Register(
            nameof(IconImageSource),
            typeof(ImageSource),
            typeof(AppTitleBar));

    public static readonly DependencyProperty ShowMaximizeProperty =
        DependencyProperty.Register(
            nameof(ShowMaximize),
            typeof(bool),
            typeof(AppTitleBar),
            new PropertyMetadata(true));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Geometry? IconGeometry
    {
        get => (Geometry?)GetValue(IconGeometryProperty);
        set => SetValue(IconGeometryProperty, value);
    }

    public ImageSource? IconImageSource
    {
        get => (ImageSource?)GetValue(IconImageSourceProperty);
        set => SetValue(IconImageSourceProperty, value);
    }

    public bool ShowMaximize
    {
        get => (bool)GetValue(ShowMaximizeProperty);
        set => SetValue(ShowMaximizeProperty, value);
    }

    public AppTitleBar()
    {
        InitializeComponent();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        Window? window = Window.GetWindow(this);
        if (window != null)
        {
            SystemCommands.MinimizeWindow(window);
        }
    }

    private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
    {
        Window? window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }

        if (window.WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(window);
        }
        else
        {
            SystemCommands.MaximizeWindow(window);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Window? window = Window.GetWindow(this);
        if (window != null)
        {
            SystemCommands.CloseWindow(window);
        }
    }
}
