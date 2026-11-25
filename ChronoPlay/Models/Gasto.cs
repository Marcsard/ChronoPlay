namespace ChronoPlay.Models;

public class Gasto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }

    public string Concepto { get; set; } = "";

    public decimal Monto { get; set; }

    public string Proveedor { get; set; } = "";
}