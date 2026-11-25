using ChronoPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Data;

public static class DatabaseBootstrapper
{
    public static void Initialize()
    {
        AppDataPaths.EnsureDirectories();

        using ChronoPlayDbContext db = new();
        db.Database.EnsureCreated();

        if (!db.Configuraciones.Any())
        {
            db.Configuraciones.Add(new Configuracion
            {
                NombreNegocio = string.Empty,
                RetencionHistorial = "30 días",
                SonidoAlarma = false
            });
            db.SaveChanges();
        }
    }
}
