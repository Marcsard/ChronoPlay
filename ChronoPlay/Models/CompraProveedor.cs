namespace ChronoPlay.Models;

public class CompraProveedor
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Proveedor { get; set; } = "";

    public string Producto { get; set; } = "";

    public int Cantidad { get; set; }

    public decimal CostoUnitario { get; set; }

    public decimal Total { get; set; }
}