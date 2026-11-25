using System.Data;
using ChronoPlay.Data;
using ChronoPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Services;

public class ProductoService
{
    public List<Producto> ObtenerTodos()
    {
        using ChronoPlayDbContext db = new();

        return db.Productos
                 .OrderBy(p => p.Nombre)
                 .ToList();
    }

    public void Agregar(Producto producto)
    {
        using ChronoPlayDbContext db = new();
#if CHRONOPLAY_PUBLISHED
        using var transactionSqlite = db.Database.BeginTransaction(IsolationLevel.Serializable);
        producto.Id = 0;
        db.Productos.Add(producto);
        db.MovimientosCaja.Add(new MovimientoCaja
        {
            Fecha = DateTime.Now,
            Tipo = "COMPRA",
            Concepto = $"{producto.Nombre} - {producto.Proveedor}",
            Monto = producto.Costo * producto.Stock,
            Naturaleza = "GASTO"
        });
        db.SaveChanges();
        transactionSqlite.Commit();
#else
        using var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable);

        List<int> idsExistentes = db.Productos
            .FromSqlRaw("SELECT * FROM dbo.Productos WITH (UPDLOCK, HOLDLOCK)")
            .OrderBy(item => item.Id)
            .Select(item => item.Id)
            .ToList();

        int idDisponible = 1;
        foreach (int id in idsExistentes)
        {
            if (id == idDisponible)
            {
                if (idDisponible == int.MaxValue)
                {
                    throw new InvalidOperationException(
                        "No hay más identificadores disponibles para productos.");
                }

                idDisponible++;
            }
            else if (id > idDisponible)
            {
                break;
            }
        }

        producto.Id = idDisponible;

        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Productos ON");
        try
        {
            db.Productos.Add(producto);
            db.MovimientosCaja.Add(new MovimientoCaja
            {
                Fecha = DateTime.Now,
                Tipo = "COMPRA",
                Concepto = $"{producto.Nombre} - {producto.Proveedor}",
                Monto = producto.Costo * producto.Stock,
                Naturaleza = "GASTO"
            });
            db.SaveChanges();
        }
        finally
        {
            db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Productos OFF");
        }

        transaction.Commit();
#endif
    }

    public void Actualizar(Producto producto)
    {
        using ChronoPlayDbContext db = new();

        db.Productos.Update(producto);

        db.SaveChanges();
    }

    public Dictionary<int, int> DescontarStock(
        IReadOnlyCollection<VentaProducto> venta)
    {
        var cantidades = venta
            .GroupBy(item => item.Producto.Id)
            .Select(grupo => new
            {
                ProductoId = grupo.Key,
                Cantidad = grupo.Sum(item => item.Cantidad)
            })
            .ToList();

        if (cantidades.Any(item => item.Cantidad <= 0))
        {
            throw new InvalidOperationException(
                "La cantidad a descontar debe ser mayor que cero.");
        }

        using ChronoPlayDbContext db = new();
        using var transaction = db.Database.BeginTransaction();

        foreach (var item in cantidades)
        {
            int filasActualizadas = db.Productos
                .Where(producto => producto.Id == item.ProductoId &&
                                   producto.Stock >= item.Cantidad)
                .ExecuteUpdate(operacion => operacion.SetProperty(
                    producto => producto.Stock,
                    producto => producto.Stock - item.Cantidad));

            if (filasActualizadas == 0)
            {
                Producto? producto = db.Productos
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == item.ProductoId);

                string mensaje = producto == null
                    ? "Uno de los productos ya no existe."
                    : $"Stock insuficiente para {producto.Nombre}. " +
                      $"Disponible: {producto.Stock}.";

                throw new InvalidOperationException(mensaje);
            }
        }

        int[] ids = cantidades.Select(item => item.ProductoId).ToArray();
        Dictionary<int, int> stockActualizado = db.Productos
            .AsNoTracking()
            .Where(producto => ids.Contains(producto.Id))
            .ToDictionary(producto => producto.Id, producto => producto.Stock);

        transaction.Commit();
        return stockActualizado;
    }

    public int DescontarStock(int productoId, int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new InvalidOperationException(
                "La cantidad a descontar debe ser mayor que cero.");
        }

        using ChronoPlayDbContext db = new();

        int filasActualizadas = db.Productos
            .Where(producto => producto.Id == productoId &&
                               producto.Stock >= cantidad)
            .ExecuteUpdate(operacion => operacion.SetProperty(
                producto => producto.Stock,
                producto => producto.Stock - cantidad));

        if (filasActualizadas == 0)
        {
            Producto? producto = db.Productos
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == productoId);

            string mensaje = producto == null
                ? "El producto seleccionado ya no existe."
                : $"Stock insuficiente para {producto.Nombre}. " +
                  $"Disponible: {producto.Stock}.";

            throw new InvalidOperationException(mensaje);
        }

        return db.Productos
            .AsNoTracking()
            .Where(producto => producto.Id == productoId)
            .Select(producto => producto.Stock)
            .Single();
    }

    public void Eliminar(int id)
    {
        using ChronoPlayDbContext db = new();

        Producto? producto =
            db.Productos.FirstOrDefault(
                p => p.Id == id);

        if (producto != null)
        {
            db.Productos.Remove(producto);

            db.SaveChanges();
        }
    }

    public Producto? ObtenerPorId(int id)
    {
        using ChronoPlayDbContext db = new();

        return db.Productos
                 .FirstOrDefault(
                     p => p.Id == id);
    }
}