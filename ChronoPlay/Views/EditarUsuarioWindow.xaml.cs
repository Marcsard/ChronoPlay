using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class EditarUsuarioWindow : Window
{
    private readonly Usuario _usuario;

    public EditarUsuarioWindow(
        Usuario usuario)
    {
        InitializeComponent();

        _usuario = usuario;

        txtNombre.Text =
            usuario.Nombre;

        txtUsuario.Text =
            usuario.UsuarioLogin;

        txtPassword.Password =
            usuario.Password;

        foreach (ComboBoxItem item in cmbRol.Items)
        {
            if (item.Content.ToString()
                == usuario.Rol)
            {
                cmbRol.SelectedItem = item;
                break;
            }
        }
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        _usuario.Nombre =
            txtNombre.Text;

        _usuario.UsuarioLogin =
            txtUsuario.Text;

        _usuario.Password =
            txtPassword.Password;

        _usuario.Rol =
            ((ComboBoxItem)cmbRol.SelectedItem!)
            .Content
            .ToString()!;

        UsuarioService service = new();

        service.Actualizar(_usuario);

        DialogResult = true;

        Close();
    }
}