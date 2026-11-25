using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class SesionService
{
    public List<Sesion> ObtenerUltimos30Dias()
    {
        using ChronoPlayDbContext db = new();

        DateTime fecha =
            DateTime.Now.AddDays(-30);

        return db.Sesiones
            .Where(x => x.Fin >= fecha)
            .OrderByDescending(x => x.Fin)
            .ToList();
    }

    public void RegistrarSesion(
        Sesion sesion)
    {
        using ChronoPlayDbContext db = new();

        db.Sesiones.Add(sesion);

        db.SaveChanges();
    }

    public void RegistrarSesionConIngreso(Sesion sesion)
    {
        using ChronoPlayDbContext db = new();
        using var transaction = db.Database.BeginTransaction();

        db.Sesiones.Add(sesion);
        db.MovimientosCaja.Add(new MovimientoCaja
        {
            Fecha = sesion.Fin,
            Tipo = "CONSOLA",
            Concepto = sesion.Consola,
            Monto = sesion.Total,
            Naturaleza = "INGRESO"
        });

        db.SaveChanges();
        transaction.Commit();
    }

    public void Eliminar(int id)
    {
        using ChronoPlayDbContext db = new();

        var sesion =
            db.Sesiones
              .FirstOrDefault(
                x => x.Id == id);

        if (sesion != null)
        {
            db.Sesiones.Remove(sesion);

            db.SaveChanges();
        }
    }
    public void LimpiarRegistrosAntiguos(
      int dias)

    {
        using ChronoPlayDbContext db = new();

        DateTime fechaLimite =
            DateTime.Now.AddDays(-dias);

        var sesiones =
            db.Sesiones
              .Where(x => x.Fin < fechaLimite)
              .ToList();

        if (sesiones.Any())
        {
            db.Sesiones.RemoveRange(sesiones);

            db.SaveChanges();
        }
    }


}