using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class AgregarConsolaWindow : Window
{
    public AgregarConsolaWindow()
    {
        InitializeComponent();
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        string tipo =
            ((ComboBoxItem)cmbTipo.SelectedItem)
            .Content.ToString()!;

        Consola consola = new()
        {
            Nombre = txtNombre.Text,
            Tipo = tipo,
            PrecioHora =
                decimal.Parse(txtPrecio.Text),
            Activa = true,
            Ocupada = false,
            TiempoLibre = true
        };

        ConsolaService service = new();

        service.Agregar(consola);

        DialogResult = true;

        Close();
    }
}
