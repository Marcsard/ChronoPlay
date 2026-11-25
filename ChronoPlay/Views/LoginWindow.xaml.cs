using System.Windows;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Ingresar_Click(object sender, RoutedEventArgs e)
    {
        LoginService loginService = new();

        var usuario =
            loginService.Login(
                txtUsuario.Text,
                txtPassword.Password);

        if (usuario != null)
        {
            DashboardWindow dashboard = new(usuario);
            Application.Current.MainWindow = dashboard;
            dashboard.Show();
            Close();
        }
        else
        {
            AppDialog.Show(
                this,
                AppDialogType.Error,
                "// ACCESO DENEGADO",
                "NO SE PUDO INICIAR SESIÓN",
                "Usuario o contraseña incorrectos.");
        }
    }
}