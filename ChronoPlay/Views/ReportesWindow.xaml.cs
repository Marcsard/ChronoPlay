using System.Windows;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class ReportesWindow : Window
{
    private readonly CajaService _cajaService = new();

    public ReportesWindow()
    {
        InitializeComponent();

        CargarAnios();

        CargarMeses();
    }

    private void CargarAnios()
    {
        for (int anio = 2025; anio <= DateTime.Now.Year; anio++)
        {
            cmbAnio.Items.Add(anio);
        }

        cmbAnio.SelectedItem =
            DateTime.Now.Year;
    }

    private void CargarMeses()
    {
        cmbMes.Items.Add("Enero");
        cmbMes.Items.Add("Febrero");
        cmbMes.Items.Add("Marzo");
        cmbMes.Items.Add("Abril");
        cmbMes.Items.Add("Mayo");
        cmbMes.Items.Add("Junio");
        cmbMes.Items.Add("Julio");
        cmbMes.Items.Add("Agosto");
        cmbMes.Items.Add("Septiembre");
        cmbMes.Items.Add("Octubre");
        cmbMes.Items.Add("Noviembre");
        cmbMes.Items.Add("Diciembre");

        cmbMes.SelectedIndex =
            DateTime.Now.Month - 1;
    }

    private void Consultar_Click(
        object sender,
        RoutedEventArgs e)
    {
        int anio =
            (int)cmbAnio.SelectedItem;

        int mes =
            cmbMes.SelectedIndex + 1;

        decimal consolas =
            _cajaService.ObtenerIngresosConsolas(
                anio,
                mes);

        decimal productos =
            _cajaService.ObtenerIngresosProductos(
                anio,
                mes);

        decimal inventario =
            _cajaService.ObtenerComprasInventario(
                anio,
                mes);

        decimal extras =
            _cajaService.ObtenerGastosExtras(
                anio,
                mes);

        decimal ingresos =
            consolas + productos;

        decimal gastos =
            inventario + extras;

        decimal ganancia =
            ingresos - gastos;

        txtConsolas.Text =
            $"🎮 Consolas: {consolas:C}";

        txtProductos.Text =
            $"🛒 Productos: {productos:C}";

        txtCompras.Text =
            $"📦 Inventario: {inventario:C}";

        txtExtras.Text =
            $"💡 Gastos Extras: {extras:C}";

        txtGanancia.Text =
            $"Ganancia: {ganancia:C}";
    }
}