namespace ChronoPlay.Models;

public class MovimientoCaja
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = "";

    public string Concepto { get; set; } = "";

    public decimal Monto { get; set; }

    public string Naturaleza { get; set; } = "";
}