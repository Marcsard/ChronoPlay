namespace ChronoPlay.Models;

public class TicketVenta
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public List<VentaProducto> Productos { get; set; }
        = new();

    public decimal Total =>
        Productos.Sum(x => x.Total);
}