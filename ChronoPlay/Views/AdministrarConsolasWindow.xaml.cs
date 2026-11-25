using System.Windows;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class AdministrarConsolasWindow : Window
{
    private readonly ConsolaService _service =
        new();

    public AdministrarConsolasWindow()
    {
        InitializeComponent();

        CargarConsolas();
    }

    private void CargarConsolas()
    {
        dgConsolas.ItemsSource = null;

        dgConsolas.ItemsSource =
            _service.ObtenerConsolas();
    }
    private void btnAgregar_Click(
        object sender,
        RoutedEventArgs e)
    {
        AgregarConsolaWindow ventana =
            new();

        if (ventana.ShowDialog() == true)
        {
            CargarConsolas();
        }
    }
    private void btnEditar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgConsolas.SelectedItem is not Consola consola)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// SELECCIÓN REQUERIDA",
                "SIN CONSOLA SELECCIONADA",
                "Seleccione una consola.");

            return;
        }

        EditarConsolaWindow ventana =
            new(consola);

        if (ventana.ShowDialog() == true)
        {
            CargarConsolas();
        }
    }
    private void btnEliminar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgConsolas.SelectedItem is not Consola consola)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// SELECCIÓN REQUERIDA",
                "SIN CONSOLA SELECCIONADA",
                "Seleccione una consola.");

            return;
        }

        bool confirmar = AppDialog.Confirm(
            this,
            "// CONFIRMAR ACCIÓN",
            "ELIMINAR CONSOLA",
            $"¿Desea eliminar {consola.Nombre}?",
            "Eliminar");

        if (confirmar)
        {
            _service.Eliminar(consola.Id);

            CargarConsolas();
        }
    }
}