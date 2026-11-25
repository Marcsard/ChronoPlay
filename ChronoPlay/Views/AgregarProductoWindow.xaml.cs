using ChronoPlay.Models;
using ChronoPlay.Services;
using System.Windows;

namespace ChronoPlay.Views;

public partial class AgregarProductoWindow : Window
{
    public Producto? ProductoCreado { get; private set; }

    public AgregarProductoWindow()
    {
        InitializeComponent();
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            Producto producto = new()
            {
                Nombre = txtNombre.Text,
                Categoria = txtCategoria.Text,
                Costo = decimal.Parse(txtCosto.Text),
                PrecioVenta = decimal.Parse(txtVenta.Text),
                Stock = int.Parse(txtStock.Text),
                Proveedor = txtProveedor.Text
            };

            ProductoService service = new();

            service.Agregar(producto);

            ProductoCreado = producto;

            DialogResult = true;

            Close();
        }
        catch (Exception ex)
        {
            AppDialog.Show(
                this,
                AppDialogType.Error,
                "// NO SE PUDO GUARDAR",
                "ERROR AL REGISTRAR PRODUCTO",
                $"Verifique los datos capturados.\n{ex.Message}");
        }
    }
}