using ChronoPlay.Data;
using ChronoPlay.Models;

namespace ChronoPlay.Services;

public class ProveedorService
{
    public List<Proveedor> ObtenerTodos()
    {
        using ChronoPlayDbContext db = new();

        return db.Productos
                 .Select(producto => producto.Proveedor)
                 .ToList()
                 .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                 .Select(nombre => nombre.Trim())
                 .Distinct(StringComparer.OrdinalIgnoreCase)
                 .OrderBy(nombre => nombre)
                 .Select(nombre => new Proveedor { Nombre = nombre })
                 .ToList();
    }

    public void Agregar(
        Proveedor proveedor)
    {
        using ChronoPlayDbContext db = new();

        db.Proveedores.Add(proveedor);

        db.SaveChanges();
    }

    public Proveedor? ObtenerPorNombre(
    string nombre)
    {
        using ChronoPlayDbContext db = new();

        return db.Proveedores
                 .FirstOrDefault(
                    p => p.Nombre == nombre);
    }
}