using ChronoPlay.Data;
using ChronoPlay.Models;
using ChronoPlay.Services;
using System.Windows;

namespace ChronoPlay.Views;

public partial class UsuariosWindow : Window
{
    private readonly UsuarioService _usuarioService;

    public UsuariosWindow()
    {
        InitializeComponent();

        _usuarioService = new UsuarioService();

        ActualizarGrid();
    }

    private void ActualizarGrid()
    {
        dgUsuarios.ItemsSource = null;

        dgUsuarios.ItemsSource =
            _usuarioService.ObtenerTodos();
    }

    private void btnAgregarUsuario_Click(
        object sender,
        RoutedEventArgs e)
    {
        AgregarUsuarioWindow ventana =
            new();

        if (ventana.ShowDialog() == true)
        {
            ActualizarGrid();
        }
    }
    private void btnEditar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgUsuarios.SelectedItem
            is not Usuario usuario)
            return;

        EditarUsuarioWindow ventana =
            new(usuario);

        if (ventana.ShowDialog() == true)
        {
            ActualizarGrid();
        }
    }
    private void btnDesactivar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgUsuarios.SelectedItem
            is not Usuario usuario)
            return;

        UsuarioService service = new();

        service.Desactivar(usuario.Id);

        ActualizarGrid();
    }
    private void btnActivar_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (dgUsuarios.SelectedItem
            is not Usuario usuario)
            return;

        UsuarioService service = new();

        service.Activar(usuario.Id);

        ActualizarGrid();
    }
    public void Activar(int id)
    {
        using ChronoPlayDbContext db = new();

        Usuario? usuario =
            db.Usuarios.FirstOrDefault(
                x => x.Id == id);

        if (usuario != null)
        {
            usuario.Activo = true;

            db.SaveChanges();
        }
    }
    public void Desactivar(int id)
    {
        using ChronoPlayDbContext db = new();

        Usuario? usuario =
            db.Usuarios.FirstOrDefault(
                x => x.Id == id);

        if (usuario != null)
        {
            usuario.Activo = false;

            db.SaveChanges();
        }
    }
}