using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class UsuarioService
{
    public List<Usuario> ObtenerTodos()
    {
        using ChronoPlayDbContext db = new();

        return db.Usuarios.ToList();
    }

    public void Agregar(Usuario usuario)
    {
        using ChronoPlayDbContext db = new();

        db.Usuarios.Add(usuario);

        db.SaveChanges();
    }
    public void Actualizar(Usuario usuario)
    {
        using ChronoPlayDbContext db = new();

        db.Usuarios.Update(usuario);

        db.SaveChanges();
    }
    public void Activar(int id)
    {
        using ChronoPlayDbContext db = new();

        var usuario =
            db.Usuarios.FirstOrDefault(
                u => u.Id == id);

        if (usuario != null)
        {
            usuario.Activo = true;

            db.SaveChanges();
        }
    }
    public void Desactivar(int id)
    {
        using ChronoPlayDbContext db = new();

        var usuario =
            db.Usuarios.FirstOrDefault(
                u => u.Id == id);

        if (usuario != null)
        {
            usuario.Activo = false;

            db.SaveChanges();
        }
    }
  

}