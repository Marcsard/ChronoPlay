using System.Windows;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class HistorialWindow : Window
{
    private readonly SesionService _sesionService = new();
    private readonly HistorialService _historialService = new();

    public HistorialWindow()
    {
        InitializeComponent();

        CargarHistorial();
    }

    private void CargarHistorial()
    {
        dgHistorial.ItemsSource = null;
        dgHistorial.ItemsSource = _historialService.ObtenerUltimos30Dias();
        btnEliminarRegistro.IsEnabled = false;
    }

    private void dgHistorial_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        btnEliminarRegistro.IsEnabled =
            dgHistorial.SelectedItem is HistorialRegistro { SesionId: not null };
    }

    private void btnEliminar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (dgHistorial.SelectedItem is not HistorialRegistro { SesionId: int sesionId } registro)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// SELECCIÓN REQUERIDA",
                "NO SE PUEDE ELIMINAR",
                "Selecciona una sesión de consola para eliminarla. Las ventas y gastos se conservan para auditoría.");
            return;
        }

        bool confirmar = AppDialog.Confirm(
            this,
            "// CONFIRMAR ACCIÓN",
            "ELIMINAR SESIÓN",
            $"¿Eliminar la sesión de {registro.Detalle}?",
            "Eliminar");

        if (confirmar)
        {
            _sesionService.Eliminar(sesionId);
            CargarHistorial();
        }
    }
}