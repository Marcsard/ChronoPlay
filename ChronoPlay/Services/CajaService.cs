using ChronoPlay.Data;
using ChronoPlay.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChronoPlay.Services;


public class CajaService
{
    public void RegistrarIngreso(
        string tipo,
        string concepto,
        decimal monto)
    {
        using ChronoPlayDbContext db = new();

        db.MovimientosCaja.Add(
            new MovimientoCaja
            {
                Fecha = DateTime.Now,
                Tipo = tipo,
                Concepto = concepto,
                Monto = monto,
                Naturaleza = "INGRESO"
            });

        db.SaveChanges();
    }

    public void RegistrarGasto(
        string tipo,
        string concepto,
        decimal monto)
    {
        using ChronoPlayDbContext db = new();

        db.MovimientosCaja.Add(
            new MovimientoCaja
            {
                Fecha = DateTime.Now,
                Tipo = tipo,
                Concepto = concepto,
                Monto = monto,
                Naturaleza = "GASTO"
            });

        db.SaveChanges();
    }

    public decimal ObtenerIngresos()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
                 .Where(x => x.Naturaleza == "INGRESO")
                 .Sum(x => x.Monto);
    }

    public decimal ObtenerIngresos(DateTime fecha)
    {
        using ChronoPlayDbContext db = new();

        return ObtenerMovimientosDelDia(db, fecha)
            .Where(movimiento => movimiento.Naturaleza == "INGRESO")
            .Sum(movimiento => movimiento.Monto);
    }

    public decimal ObtenerGastos()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
                 .Where(x => x.Naturaleza == "GASTO")
                 .Sum(x => x.Monto);
    }

    public decimal ObtenerGanancia()
    {
        return ObtenerIngresos()
             - ObtenerGastos();
    }

    public List<MovimientoCaja> ObtenerMovimientos()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
                 .OrderByDescending(x => x.Fecha)
                 .ToList();
    }
    public decimal ObtenerIngresosConsolas()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "CONSOLA")
            .Sum(x => x.Monto);
    }

    public decimal ObtenerIngresosConsolas(DateTime fecha)
    {
        using ChronoPlayDbContext db = new();

        return ObtenerMovimientosDelDia(db, fecha)
            .Where(movimiento => movimiento.Tipo == "CONSOLA")
            .Sum(movimiento => movimiento.Monto);
    }

    public decimal ObtenerIngresosProductos()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "PRODUCTO")
            .Sum(x => x.Monto);
    }

    public decimal ObtenerIngresosProductos(DateTime fecha)
    {
        using ChronoPlayDbContext db = new();

        return ObtenerMovimientosDelDia(db, fecha)
            .Where(movimiento => movimiento.Tipo == "PRODUCTO")
            .Sum(movimiento => movimiento.Monto);
    }

    public decimal ObtenerComprasInventario()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "COMPRA")
            .Sum(x => x.Monto);
    }

    public decimal ObtenerComprasInventario(DateTime fecha)
    {
        using ChronoPlayDbContext db = new();

        return ObtenerMovimientosDelDia(db, fecha)
            .Where(movimiento => movimiento.Tipo == "COMPRA")
            .Sum(movimiento => movimiento.Monto);
    }

    public decimal ObtenerGastosExtras()
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "GASTO_EXTRA")
            .Sum(x => x.Monto);
    }

    public decimal ObtenerGastosExtras(DateTime fecha)
    {
        using ChronoPlayDbContext db = new();

        return ObtenerMovimientosDelDia(db, fecha)
            .Where(movimiento => movimiento.Tipo == "GASTO_EXTRA")
            .Sum(movimiento => movimiento.Monto);
    }

    public decimal ObtenerIngresosConsolas(
    int anio,
    int mes)
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "CONSOLA" &&
                x.Fecha.Year == anio &&
                x.Fecha.Month == mes)
            .Sum(x => x.Monto);
    }
    public decimal ObtenerIngresosProductos(
    int anio,
    int mes)
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "PRODUCTO" &&
                x.Fecha.Year == anio &&
                x.Fecha.Month == mes)
            .Sum(x => x.Monto);
    }
    public decimal ObtenerComprasInventario(
    int anio,
    int mes)
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "COMPRA" &&
                x.Fecha.Year == anio &&
                x.Fecha.Month == mes)
            .Sum(x => x.Monto);
    }
    public decimal ObtenerGastosExtras(
    int anio,
    int mes)
    {
        using ChronoPlayDbContext db = new();

        return db.MovimientosCaja
            .Where(x =>
                x.Tipo == "GASTO_EXTRA" &&
                x.Fecha.Year == anio &&
                x.Fecha.Month == mes)
            .Sum(x => x.Monto);
    }

    private static IQueryable<MovimientoCaja> ObtenerMovimientosDelDia(
        ChronoPlayDbContext db,
        DateTime fecha)
    {
        DateTime inicio = fecha.Date;
        DateTime fin = inicio.AddDays(1);

        return db.MovimientosCaja
            .Where(movimiento => movimiento.Fecha >= inicio &&
                                 movimiento.Fecha < fin);
    }
}