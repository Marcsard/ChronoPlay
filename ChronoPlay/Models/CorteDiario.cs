namespace ChronoPlay.Models;

public class CorteDiario
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal VentasConsolas { get; set; }

    public decimal VentasProductos { get; set; }

    public decimal Gastos { get; set; }

    public decimal Total { get; set; }
}