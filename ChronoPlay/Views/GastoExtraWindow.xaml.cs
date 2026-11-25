using System.Windows;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class GastoExtraWindow : Window
{
    public GastoExtraWindow()
    {
        InitializeComponent();
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        decimal monto =
            decimal.Parse(txtMonto.Text);

        if (!AppDialog.Confirm(
                this,
                "// CONFIRMAR GASTO",
                "¿REGISTRAR ESTE GASTO?",
                $"Concepto: {txtConcepto.Text}\nMonto: {monto:C}",
                "Guardar gasto"))
        {
            return;
        }

        CajaService caja = new();

        caja.RegistrarGasto(
            "GASTO_EXTRA",
            txtConcepto.Text,
            monto);

        DialogResult = true;

        Close();
    }
}