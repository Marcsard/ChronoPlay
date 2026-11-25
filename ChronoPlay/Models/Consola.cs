public class Consola
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Tipo { get; set; } = "";

    public decimal PrecioHora { get; set; }

    public bool Activa { get; set; } = true;

    public bool Ocupada { get; set; }

    public DateTime HoraInicio { get; set; }

    public TimeSpan TiempoJugado { get; set; }

    public TimeSpan TiempoObjetivo { get; set; }

    public bool TiempoLibre { get; set; }
}