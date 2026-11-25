using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class AgregarUsuarioWindow : Window
{
    public AgregarUsuarioWindow()
    {
        InitializeComponent();
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (cmbRol.SelectedItem == null)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "ROL REQUERIDO",
                "Seleccione un rol.");

            return;
        }

        string rol =
            ((ComboBoxItem)cmbRol.SelectedItem)
            .Content.ToString()!;

        UsuarioService usuarioService = new();

        List<Usuario> usuarios =
            usuarioService.ObtenerTodos();

        if (rol == "Administrador" &&
            usuarios.Count(x =>
                x.Rol == "Administrador") >= 2)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// LÍMITE ALCANZADO",
                "LÍMITE DE ADMINISTRADORES",
                "Solo se permiten 2 administradores.");

            return;
        }

        if (rol == "Empleado" &&
            usuarios.Count(x =>
                x.Rol == "Empleado") >= 5)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// LÍMITE ALCANZADO",
                "LÍMITE DE EMPLEADOS",
                "Solo se permiten 5 empleados.");

            return;
        }

        Usuario usuario = new()
        {
            Nombre = txtNombre.Text,
            UsuarioLogin = txtUsuario.Text,
            Password = txtPassword.Password,
            Rol = rol,
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        usuarioService.Agregar(usuario);

        DialogResult = true;

        Close();
    }
}
