using ChronoPlay.Data;
using ChronoPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Services;

public sealed class HistorialService
{
    public List<HistorialRegistro> ObtenerUltimos30Dias()
    {
        DateTime desde = DateTime.Now.AddDays(-30);
        using ChronoPlayDbContext db = new();

        List<Sesion> sesiones = db.Sesiones
            .AsNoTracking()
            .Where(sesion => sesion.Fin >= desde)
            .ToList();

        List<MovimientoCaja> movimientos = db.MovimientosCaja
            .AsNoTracking()
            .Where(movimiento => movimiento.Fecha >= desde && movimiento.Tipo != "CONSOLA")
            .ToList();

        List<Venta> ventas = db.Ventas
            .AsNoTracking()
            .Where(venta => venta.Fecha >= desde)
            .ToList();

        List<HistorialRegistro> registros = sesiones
            .Select(sesion => new HistorialRegistro
            {
                Id = $"SES-{sesion.Id}",
                Tipo = "SESIÓN DE CONSOLA",
                Detalle = sesion.Consola,
                Fecha = sesion.Fin.ToString("dd/MM/yyyy HH:mm"),
                Inicio = sesion.Inicio.ToString("dd/MM/yyyy HH:mm"),
                Fin = sesion.Fin.ToString("dd/MM/yyyy HH:mm"),
                TiempoJugado = sesion.TiempoJugado.ToString(@"hh\:mm\:ss"),
                Total = sesion.Total.ToString("C"),
                FechaOrden = sesion.Fin,
                SesionId = sesion.Id
            })
            .ToList();

        foreach (MovimientoCaja movimiento in movimientos)
        {
            bool esIngreso = movimiento.Naturaleza == "INGRESO";
            string tipo = movimiento.Tipo switch
            {
                "PRODUCTO" => "VENTA",
                "COMPRA" => "COMPRA DE INVENTARIO",
                "GASTO_EXTRA" => "GASTO",
                _ => esIngreso ? "INGRESO" : "GASTO"
            };

            registros.Add(new HistorialRegistro
            {
                Id = $"MOV-{movimiento.Id}",
                Tipo = tipo,
                Detalle = movimiento.Concepto,
                Fecha = movimiento.Fecha.ToString("dd/MM/yyyy HH:mm"),
                Total = (esIngreso ? movimiento.Monto : -movimiento.Monto).ToString("C"),
                FechaOrden = movimiento.Fecha
            });
        }

        foreach (Venta venta in ventas)
        {
            bool yaRegistradaComoMovimiento = movimientos.Any(movimiento =>
                movimiento.Naturaleza == "INGRESO" &&
                movimiento.Tipo == venta.Tipo &&
                movimiento.Concepto == venta.Concepto &&
                movimiento.Monto == venta.Total &&
                movimiento.Fecha == venta.Fecha);

            if (!yaRegistradaComoMovimiento)
            {
                registros.Add(new HistorialRegistro
                {
                    Id = $"VEN-{venta.Id}",
                    Tipo = "VENTA",
                    Detalle = venta.Concepto,
                    Fecha = venta.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Total = venta.Total.ToString("C"),
                    FechaOrden = venta.Fecha
                });
            }
        }

        return registros
            .OrderByDescending(registro => registro.FechaOrden)
            .ToList();
    }
}
