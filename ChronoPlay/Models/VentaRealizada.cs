namespace ChronoPlay.Models;

public class VentaRealizada
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public int ProductosVendidos { get; set; }
}
