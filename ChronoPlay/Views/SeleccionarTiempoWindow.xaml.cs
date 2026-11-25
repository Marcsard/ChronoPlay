using System.Windows;

namespace ChronoPlay.Views;

public partial class SeleccionarTiempoWindow : Window
{
    public TimeSpan TiempoSeleccionado
    {
        get;
        private set;
    }

    public bool TiempoLibre
    {
        get;
        private set;
    }

    public SeleccionarTiempoWindow()
    {
        InitializeComponent();
    }

    private void Aceptar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (rb30.IsChecked == true)
        {
            TiempoSeleccionado =
                TimeSpan.FromMinutes(30);
        }
        else if (rb60.IsChecked == true)
        {
            TiempoSeleccionado =
                TimeSpan.FromHours(1);
        }
        else if (rb120.IsChecked == true)
        {
            TiempoSeleccionado =
                TimeSpan.FromHours(2);
        }
        else
        {
            TiempoLibre = true;
        }

        DialogResult = true;

        Close();
    }
}