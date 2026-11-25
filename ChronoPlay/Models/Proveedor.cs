namespace ChronoPlay.Models;

public class Proveedor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Telefono { get; set; } = "";

    public string Observaciones { get; set; } = "";

    public override string ToString()
    {
        return Nombre;
    }
}