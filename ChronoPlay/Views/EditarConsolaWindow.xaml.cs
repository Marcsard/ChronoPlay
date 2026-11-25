using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class EditarConsolaWindow : Window
{
    private readonly Consola _consola;

    public EditarConsolaWindow(
        Consola consola)
    {
        InitializeComponent();

        _consola = consola;

        txtNombre.Text =
            consola.Nombre;

        txtPrecio.Text =
            consola.PrecioHora.ToString();

        foreach (ComboBoxItem item in cmbTipo.Items)
        {
            if (item.Content.ToString()
                == consola.Tipo)
            {
                cmbTipo.SelectedItem = item;
                break;
            }
        }
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        string tipo =
            ((ComboBoxItem)cmbTipo.SelectedItem)
            .Content.ToString()!;

        _consola.Nombre =
            txtNombre.Text;

        _consola.Tipo =
            tipo;

        _consola.PrecioHora =
            decimal.Parse(txtPrecio.Text);

        ConsolaService service = new();

        service.Actualizar(_consola);

        DialogResult = true;

        Close();
    }
}