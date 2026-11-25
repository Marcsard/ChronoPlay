using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChronoPlay.Controls;

namespace ChronoPlay.Views;

public partial class AppDialogWindow : Window
{
    internal AppDialogWindow(AppDialogModel model, Window? owner)
    {
        InitializeComponent();
        DataContext = model;

        if (owner is not null)
        {
            Owner = owner;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            new Action(() => btnAccept.Focus()));
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Aceptar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}

public enum AppDialogType
{
    Success,
    Error,
    Warning,
    Information
}

public static class AppDialog
{
    public static bool Show(
        Window? owner,
        AppDialogType type,
        string header,
        string title,
        string message,
        string buttonText = "Aceptar") =>
        Show(owner, type, header, title, null, null, null, message, buttonText);

    public static bool Show(
        Window? owner,
        AppDialogType type,
        string header,
        string title,
        IEnumerable<(string Label, string Value)>? rows = null,
        string? totalLabel = null,
        string? totalValue = null,
        string? message = null,
        string buttonText = "Aceptar")
    {
        AppDialogModel model = CreateModel(
            type,
            header,
            title,
            rows,
            totalLabel,
            totalValue,
            message,
            buttonText,
            null,
            false);

        return new AppDialogWindow(model, owner).ShowDialog() == true;
    }

    public static bool Confirm(
        Window? owner,
        string header,
        string title,
        string message,
        string confirmText = "Confirmar",
        string cancelText = "Cancelar")
    {
        AppDialogModel model = CreateModel(
            AppDialogType.Warning,
            header,
            title,
            null,
            null,
            null,
            message,
            confirmText,
            cancelText,
            true);

        return new AppDialogWindow(model, owner).ShowDialog() == true;
    }

    private static AppDialogModel CreateModel(
        AppDialogType type,
        string header,
        string title,
        IEnumerable<(string Label, string Value)>? rows,
        string? totalLabel,
        string? totalValue,
        string? message,
        string buttonText,
        string? cancelText,
        bool isConfirmation)
    {
        (string geometry, string brushKey) = type switch
        {
            AppDialogType.Success => ("M4,12 L9,17 L20,6", "SuccessGlow"),
            AppDialogType.Error => ("M6,6 L18,18 M18,6 L6,18", "DangerGlow"),
            AppDialogType.Warning => ("M12,4 L12,13 M12,17 L12.1,17", "Price"),
            _ => ("M12,11 L12,17 M12,7 L12.1,7", "Cyan")
        };

        Brush accent = (Brush)Application.Current.FindResource(brushKey);
        AppDialogModel model = new()
        {
            Header = header,
            Title = title,
            Message = message ?? string.Empty,
            MessageVisibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible,
            Rows = rows?.Select(row => new AppDialogRow(row.Label, row.Value)).ToList()
                ?? new List<AppDialogRow>(),
            TotalLabel = totalLabel ?? string.Empty,
            TotalValue = totalValue ?? string.Empty,
            TotalVisibility = string.IsNullOrWhiteSpace(totalLabel) || string.IsNullOrWhiteSpace(totalValue)
                ? Visibility.Collapsed
                : Visibility.Visible,
            IconGeometry = Geometry.Parse(geometry),
            AccentBrush = accent,
            ButtonText = buttonText,
            CancelText = cancelText ?? string.Empty,
            CancelVisibility = isConfirmation ? Visibility.Visible : Visibility.Collapsed,
            IsSingleButton = !isConfirmation
        };

        return model;
    }
}

public sealed class AppDialogModel
{
    public string Header { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Visibility MessageVisibility { get; init; }
    public IReadOnlyList<AppDialogRow> Rows { get; init; } = Array.Empty<AppDialogRow>();
    public string TotalLabel { get; init; } = string.Empty;
    public string TotalValue { get; init; } = string.Empty;
    public Visibility TotalVisibility { get; init; }
    public Geometry IconGeometry { get; init; } = Geometry.Empty;
    public Brush AccentBrush { get; init; } = Brushes.White;
    public string ButtonText { get; init; } = "Aceptar";
    public string CancelText { get; init; } = string.Empty;
    public Visibility CancelVisibility { get; init; }
    public bool IsSingleButton { get; init; }
}

public sealed record AppDialogRow(string Label, string Value);
