public class Venta
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = "";

    public string Concepto { get; set; } = "";

    public decimal Total { get; set; }

    public string Usuario { get; set; } = "";
}