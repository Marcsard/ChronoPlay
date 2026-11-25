using System.Windows;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class CorteCajaWindow : Window
{
    public CorteCajaWindow()
    {
        InitializeComponent();

        CajaService caja = new();
        DateTime hoy = DateTime.Today;

        decimal ingresosConsolas =
            caja.ObtenerIngresosConsolas(hoy);

        decimal ingresosProductos =
            caja.ObtenerIngresosProductos(hoy);

        decimal comprasInventario =
            caja.ObtenerComprasInventario(hoy);

        decimal gastosExtras =
            caja.ObtenerGastosExtras(hoy);

        decimal totalIngresos =
            ingresosConsolas +
            ingresosProductos;

        decimal totalGastos =
            comprasInventario +
            gastosExtras;

        decimal ganancia =
            totalIngresos -
            totalGastos;

        txtConsolas.Text =
            ingresosConsolas.ToString("C");

        txtProductos.Text =
            ingresosProductos.ToString("C");

        txtInventario.Text =
            comprasInventario.ToString("C");

        txtExtras.Text =
            gastosExtras.ToString("C");

        txtTotalIngresos.Text =
            totalIngresos.ToString("C");

        txtTotalGastos.Text =
            totalGastos.ToString("C");

        txtGanancia.Text =
            ganancia.ToString("C");
    }
}