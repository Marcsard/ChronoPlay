using System.Windows;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class InitialAdminWindow : Window
{
    public InitialAdminWindow()
    {
        InitializeComponent();
    }

    private void CrearCuenta_Click(object sender, RoutedEventArgs e)
    {
        string usuarioLogin = txtUsuario.Text.Trim();
        string password = txtPassword.Password;

        if (string.IsNullOrWhiteSpace(usuarioLogin))
        {
            MostrarValidacion("Escribe el nombre de usuario.");
            return;
        }

        if (password.Length < 8)
        {
            MostrarValidacion("La contraseña debe tener al menos 8 caracteres.");
            return;
        }

        if (password != txtConfirmarPassword.Password)
        {
            MostrarValidacion("Las contraseñas no coinciden.");
            return;
        }

        try
        {
            new UsuarioService().Agregar(new Usuario
            {
                Nombre = "Administrador",
                UsuarioLogin = usuarioLogin,
                Password = password,
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = DateTime.Now
            });

            DialogResult = true;
        }
        catch (Exception exception)
        {
            try
            {
                StartupLog.Write(exception);
            }
            catch (Exception logException)
            {
                System.Diagnostics.Trace.WriteLine($"No se pudo escribir el log de inicio: {logException}");
            }

            AppDialog.Show(
                this,
                AppDialogType.Error,
                "// NO SE PUDO CREAR LA CUENTA",
                "ERROR AL GUARDAR",
                "No se pudo guardar la cuenta Administrador. El detalle se registró en el log de inicio.");
        }
    }

    private void MostrarValidacion(string mensaje)
    {
        AppDialog.Show(
            this,
            AppDialogType.Warning,
            "// CONFIGURACIÓN INICIAL",
            "REVISA LOS DATOS",
            mensaje);
    }
}
