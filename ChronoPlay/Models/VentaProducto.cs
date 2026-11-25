namespace ChronoPlay.Models;

public class VentaProducto
{
    public int Id { get; set; }

    public Producto Producto { get; set; } = new();

    public int Cantidad { get; set; }

    public decimal Total =>
        Producto.PrecioVenta * Cantidad;
}