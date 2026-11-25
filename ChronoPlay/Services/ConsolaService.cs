using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class ConsolaService
{
    public List<Consola> ObtenerConsolas()
    {
        using ChronoPlayDbContext db = new();

        return db.Consolas
                 .Where(c => c.Activa)
                 .OrderBy(c => c.Nombre)
                 .ToList();
    }

    public void Agregar(Consola consola)
    {
        using ChronoPlayDbContext db = new();

        db.Consolas.Add(consola);

        db.SaveChanges();
    }

    public void Actualizar(Consola consola)
    {
        using ChronoPlayDbContext db = new();

        db.Consolas.Update(consola);

        db.SaveChanges();
    }

    public void Eliminar(int id)
    {
        using ChronoPlayDbContext db = new();

        var consola = db.Consolas
                        .FirstOrDefault(c => c.Id == id);

        if (consola != null)
        {
            db.Consolas.Remove(consola);

            db.SaveChanges();
        }
    }
}