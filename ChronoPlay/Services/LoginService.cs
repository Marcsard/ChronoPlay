using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class LoginService
{
    public Usuario? Login(
        string usuario,
        string password)
    {
        using ChronoPlayDbContext db = new();

        return db.Usuarios
            .FirstOrDefault(x =>
                x.UsuarioLogin == usuario &&
                x.Password == password &&
                x.Activo);
    }
}