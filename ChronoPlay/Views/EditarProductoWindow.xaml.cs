using System.Windows;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class EditarProductoWindow : Window
{
    private readonly Producto _producto;

    public EditarProductoWindow(
        Producto producto)
    {
        InitializeComponent();

        _producto = producto;

        txtNombre.Text = producto.Nombre;
        txtCategoria.Text = producto.Categoria;
        txtCosto.Text = producto.Costo.ToString();
        txtVenta.Text = producto.PrecioVenta.ToString();
        txtStock.Text = producto.Stock.ToString();
        txtProveedor.Text = producto.Proveedor;
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        _producto.Nombre = txtNombre.Text;
        _producto.Categoria = txtCategoria.Text;
        _producto.Costo =
            decimal.Parse(txtCosto.Text);

        _producto.PrecioVenta =
            decimal.Parse(txtVenta.Text);

        _producto.Stock =
            int.Parse(txtStock.Text);

        _producto.Proveedor =
            txtProveedor.Text;

        ProductoService service = new();

        service.Actualizar(_producto);

        DialogResult = true;

        Close();
    }
}