using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class ConfiguracionService
{
    public Configuracion Obtener()
    {
        using ChronoPlayDbContext db = new();

        Configuracion? config =
            db.Configuraciones.FirstOrDefault();

        if (config == null)
        {
            config = new Configuracion
            {
                NombreNegocio = string.Empty,
                RetencionHistorial = "30 días",
                SonidoAlarma = false
            };

            db.Configuraciones.Add(config);

            db.SaveChanges();
        }

        return config;
    }

    public void Guardar(
        Configuracion configuracion)
    {
        using ChronoPlayDbContext db = new();

        db.Configuraciones.Update(
            configuracion);

        db.SaveChanges();
    }
}