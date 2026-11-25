using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class InventarioWindow : Window
{
    private readonly ProductoService _productoService;

    public InventarioWindow()
    {
        InitializeComponent();

        _productoService = new ProductoService();

        CargarProductos();
    }

    private void CargarProductos()
    {
        dgProductos.ItemsSource = null;

        dgProductos.ItemsSource =
            _productoService.ObtenerTodos();
    }

    private void btnAgregar_Click(
        object sender,
        RoutedEventArgs e)
    {
        AgregarProductoWindow ventana = new()
        {
            Owner = this
        };

        if (ventana.ShowDialog() == true && ventana.ProductoCreado is Producto producto)
        {
            CargarProductos();

            AppDialog.Show(
                this,
                AppDialogType.Success,
                "// COMPRA REGISTRADA",
                "COMPRA REALIZADA",
                new[]
                {
                    ("PROVEEDOR", producto.Proveedor),
                    ("PRODUCTO", producto.Nombre),
                    ("CANTIDAD", producto.Stock.ToString()),
                    ("COSTO UNITARIO", producto.Costo.ToString("C"))
                },
                "TOTAL DE LA COMPRA",
                (producto.Costo * producto.Stock).ToString("C"));
        }
    }

    private void btnEliminar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgProductos.SelectedItem == null)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// SELECCIÓN REQUERIDA",
                "SIN PRODUCTO SELECCIONADO",
                "Seleccione un producto.");

            return;
        }

        Producto producto =
            (Producto)dgProductos.SelectedItem;

        bool confirmar = AppDialog.Confirm(
            this,
            "// CONFIRMAR ACCIÓN",
            "ELIMINAR PRODUCTO",
            $"¿Deseas eliminar '{producto.Nombre}'?",
            "Eliminar");

        if (confirmar)
        {
            ProductoService service = new();

            service.Eliminar(producto.Id);

            CargarProductos();

            AppDialog.Show(
                this,
                AppDialogType.Success,
                "// INVENTARIO ACTUALIZADO",
                "PRODUCTO ELIMINADO",
                "El producto se eliminó correctamente.");
        }
    }
    private void btnEditar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (dgProductos.SelectedItem == null)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// SELECCIÓN REQUERIDA",
                "SIN PRODUCTO SELECCIONADO",
                "Seleccione un producto.");

            return;
        }

        Producto producto =
            (Producto)dgProductos.SelectedItem;

        EditarProductoWindow ventana =
            new(producto);

        if (ventana.ShowDialog() == true)
        {
            CargarProductos();
        }
    }
    private void txtBuscar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        string filtro =
            txtBuscar.Text.Trim()
            .ToLower();

        ProductoService service = new();

        var productos =
            service.ObtenerTodos();

        var filtrados =
     productos.Where(p =>
         p.Nombre.ToLower().Contains(filtro)
         ||
         p.Categoria.ToLower().Contains(filtro)
         ||
         p.Proveedor.ToLower().Contains(filtro))
     .ToList();

        dgProductos.ItemsSource = null;

        dgProductos.ItemsSource = filtrados;
    }
}