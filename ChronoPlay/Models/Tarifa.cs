namespace ChronoPlay.Models;

public class Tarifa
{
    public int Id { get; set; }

    public string TipoConsola { get; set; } = "";

    public decimal PrecioHora { get; set; }
}
