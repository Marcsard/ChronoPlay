using ChronoPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoPlay.Data;

public class ChronoPlayDbContext : DbContext
{
    public ChronoPlayDbContext()
    {
    }

    public ChronoPlayDbContext(DbContextOptions<ChronoPlayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Configuracion> Configuraciones { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<Consola> Consolas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Venta> Ventas { get; set; }

    public DbSet<Producto> Productos { get; set; }

    public DbSet<Proveedor> Proveedores { get; set; }

    public DbSet<CompraProveedor> Compras { get; set; }

    public DbSet<MovimientoCaja> MovimientosCaja { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

#if CHRONOPLAY_PUBLISHED
        optionsBuilder.UseSqlite($"Data Source={AppDataPaths.DatabaseFile};Foreign Keys=True;Pooling=True");
#else
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;
              Database=ChronoPlayDB;
              Trusted_Connection=True;
              TrustServerCertificate=True;");
#endif
    }
}