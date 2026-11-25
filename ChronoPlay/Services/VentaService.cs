using ChronoPlay.Data;
using ChronoPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Services;

public class VentaService
{
    public Venta RegistrarVenta(
        int productoId,
        int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new InvalidOperationException(
                "La cantidad a vender debe ser mayor que cero.");
        }

        using ChronoPlayDbContext db = new();
        using var transaction = db.Database.BeginTransaction();

        Producto? producto = db.Productos
            .FirstOrDefault(item => item.Id == productoId);

        if (producto == null)
        {
            throw new InvalidOperationException(
                "El producto seleccionado ya no existe.");
        }

        if (producto.PrecioVenta < 0)
        {
            throw new InvalidOperationException(
                "El precio de venta del producto no puede ser negativo.");
        }

        int filasActualizadas = db.Productos
            .Where(item => item.Id == productoId && item.Stock >= cantidad)
            .ExecuteUpdate(operacion => operacion.SetProperty(
                item => item.Stock,
                item => item.Stock - cantidad));

        if (filasActualizadas == 0)
        {
            int? stockDisponible = db.Productos
                .Where(item => item.Id == productoId)
                .Select(item => (int?)item.Stock)
                .FirstOrDefault();

            string mensaje = stockDisponible == null
                ? "El producto seleccionado ya no existe."
                : $"Stock insuficiente para {producto.Nombre}. " +
                  $"Disponible: {stockDisponible.Value}.";

            throw new InvalidOperationException(mensaje);
        }

        DateTime fecha = DateTime.Now;
        Venta venta = new()
        {
            Fecha = fecha,
            Tipo = "PRODUCTO",
            Concepto = producto.Nombre,
            Total = Math.Round(producto.PrecioVenta * cantidad, 2)
        };

        db.Ventas.Add(venta);
        db.MovimientosCaja.Add(new MovimientoCaja
        {
            Fecha = fecha,
            Tipo = venta.Tipo,
            Concepto = venta.Concepto,
            Monto = venta.Total,
            Naturaleza = "INGRESO"
        });

        db.SaveChanges();
        transaction.Commit();

        return venta;
    }

    public List<Venta> ObtenerVentas()
    {
        using ChronoPlayDbContext db = new();

        return db.Ventas
                 .OrderByDescending(v => v.Fecha)
                 .ToList();
    }
}