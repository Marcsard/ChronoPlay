namespace ChronoPlay.Models;

public class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Categoria { get; set; } = "";

    public decimal Costo { get; set; }

    public decimal PrecioVenta { get; set; }

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    public string Proveedor { get; set; } = "";

    public override string ToString()
    {
        return Nombre;
    }
}