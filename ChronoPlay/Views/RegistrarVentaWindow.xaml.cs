using System.Windows;
using System.Windows.Input;
using ChronoPlay.Models;
using ChronoPlay.Services;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Views;

public partial class RegistrarVentaWindow : Window
{
    private readonly ProductoService _productoService = new();
    private readonly ProveedorService _proveedorService = new();
    private readonly VentaService _ventaService = new();
    private List<Producto> _productos = new();

    public VentaProducto? VentaRegistrada
    {
        get;
        private set;
    }

    public RegistrarVentaWindow()
    {
        InitializeComponent();
        DataObject.AddPastingHandler(txtCantidad, txtCantidad_Pasting);
        CargarDatos();
    }

    private void txtCantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsAsciiDigit);
    }

    private void txtCantidad_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        object? contenido = e.DataObject.GetDataPresent(DataFormats.UnicodeText, true)
            ? e.DataObject.GetData(DataFormats.UnicodeText)
            : e.DataObject.GetData(DataFormats.Text);

        if (contenido is not string texto ||
            !texto.All(char.IsAsciiDigit))
        {
            e.CancelCommand();
        }
    }

    private void CargarDatos()
    {
        _productos = _productoService.ObtenerTodos();
        cmbProveedor.ItemsSource = _proveedorService.ObtenerTodos();
        cmbProducto.ItemsSource = null;
        cmbProducto.IsEnabled = false;
    }

    private void cmbProveedor_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        cmbProducto.SelectedItem = null;

        if (cmbProveedor.SelectedItem is not Proveedor proveedor)
        {
            cmbProducto.ItemsSource = null;
            cmbProducto.IsEnabled = false;
            return;
        }

        cmbProducto.ItemsSource = _productos
            .Where(producto => string.Equals(
                producto.Proveedor.Trim(),
                proveedor.Nombre,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(producto => producto.Nombre)
            .ToList();

        cmbProducto.IsEnabled = cmbProducto.Items.Count > 0;
    }

    private void RegistrarVenta_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (cmbProveedor.SelectedItem is not Proveedor proveedor)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "PROVEEDOR REQUERIDO",
                "Seleccione un proveedor.");
            return;
        }

        if (cmbProducto.SelectedItem is not Producto producto ||
            !string.Equals(
                producto.Proveedor.Trim(),
                proveedor.Nombre,
                StringComparison.OrdinalIgnoreCase))
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "PRODUCTO REQUERIDO",
                "Seleccione un producto del proveedor elegido.");
            return;
        }

        if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "CANTIDAD NO VÁLIDA",
                "Ingrese una cantidad válida mayor que cero.");
            return;
        }

        if (producto.PrecioVenta < 0)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "PRECIO NO VÁLIDO",
                "El precio de venta del producto no puede ser negativo.");
            return;
        }

        try
        {
            _ventaService.RegistrarVenta(
                producto.Id,
                cantidad);

            VentaRegistrada = new VentaProducto
            {
                Producto = producto,
                Cantidad = cantidad
            };

            DialogResult = true;

            Close();
        }
        catch (DbUpdateException ex)
        {
            AppDialog.Show(
                this,
                AppDialogType.Error,
                "// ERROR DE BASE DE DATOS",
                "NO SE PUDO REGISTRAR LA VENTA",
                $"No se pudo guardar la venta.\n{ex.GetBaseException().Message}");
        }
        catch (InvalidOperationException ex)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VENTA NO REGISTRADA",
                "NO SE PUDO COMPLETAR LA VENTA",
                ex.Message);
        }
    }
}