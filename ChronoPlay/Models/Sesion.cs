namespace ChronoPlay.Models;

public class Sesion
{
    public int Id { get; set; }

    public string Consola { get; set; } = "";

    public DateTime Inicio { get; set; }

    public DateTime Fin { get; set; }

    public TimeSpan TiempoJugado { get; set; }

    public decimal Total { get; set; }
}