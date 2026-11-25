using System.Windows;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class VentasWindow : Window
{
    private readonly List<Producto> _productos;
    private readonly List<VentaProducto> _ticket = new();
    private readonly ProductoService _productoService = new();

    public VentasWindow(List<Producto> productos)
    {
        InitializeComponent();

        _productos = productos;

        cmbProductos.ItemsSource = _productos;

        ActualizarTicket();
    }

    private void ActualizarTicket()
    {
        dgTicket.ItemsSource = null;
        dgTicket.ItemsSource = _ticket;

        txtTotal.Text =
            $"Total: ${_ticket.Sum(x => x.Total):F2}";
    }
    private void Agregar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (cmbProductos.SelectedItem is not Producto producto)
        {
            AppDialog.Show(this, AppDialogType.Warning, "// VALIDACIÓN", "PRODUCTO REQUERIDO", "Seleccione un producto.");
            return;
        }

        if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
        {
            AppDialog.Show(this, AppDialogType.Warning, "// VALIDACIÓN", "CANTIDAD NO VÁLIDA", "Ingrese una cantidad válida mayor que cero.");
            return;
        }

        if (cantidad > producto.Stock)
        {
            AppDialog.Show(this, AppDialogType.Warning, "// INVENTARIO", "STOCK INSUFICIENTE", $"Stock disponible: {producto.Stock}");
            return;
        }

        var existente = _ticket.FirstOrDefault(
            item => item.Producto.Id == producto.Id);

        if (existente != null)
        {
            existente.Cantidad += cantidad;

            if (existente.Cantidad > producto.Stock)
            {
                existente.Cantidad -= cantidad;
                AppDialog.Show(this, AppDialogType.Warning, "// INVENTARIO", "STOCK INSUFICIENTE", $"Stock disponible: {producto.Stock}");
                return;
            }
        }
        else
        {
            _ticket.Add(new VentaProducto
            {
                Producto = producto,
                Cantidad = cantidad
            });
        }

        txtCantidad.Clear();
        ActualizarTicket();
    }

    private void EliminarProducto_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (dgTicket.SelectedItem is not VentaProducto item)
        {
            AppDialog.Show(this, AppDialogType.Warning, "// VALIDACIÓN", "PRODUCTO NO SELECCIONADO", "Seleccione un producto del ticket.");
            return;
        }

        _ticket.Remove(item);
        ActualizarTicket();
    }
    private void Cobrar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_ticket.Count == 0)
        {
            AppDialog.Show(this, AppDialogType.Warning, "// VALIDACIÓN", "TICKET VACÍO", "Agregue productos al ticket antes de cobrar.");
            return;
        }

        decimal totalVenta = _ticket.Sum(item => item.Total);
        Dictionary<int, int> stockActualizado;

        try
        {
            stockActualizado = _productoService.DescontarStock(_ticket);
        }
        catch (InvalidOperationException ex)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VENTA NO COMPLETADA",
                "NO SE PUDO COMPLETAR LA VENTA",
                ex.Message);
            return;
        }

        foreach (var item in _ticket)
        {
            item.Producto.Stock = stockActualizado[item.Producto.Id];
        }

        cmbProductos.Items.Refresh();

        AppDialog.Show(
            this,
            AppDialogType.Success,
            "// VENTA REGISTRADA",
            "VENTA REALIZADA",
            new[]
            {
                ("PRODUCTOS", _ticket.Count.ToString()),
                ("UNIDADES", _ticket.Sum(item => item.Cantidad).ToString())
            },
            "TOTAL",
            totalVenta.ToString("C"));

        _ticket.Clear();
        ActualizarTicket();
    }
}