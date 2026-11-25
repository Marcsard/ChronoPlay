namespace ChronoPlay.Models;

public sealed class HistorialRegistro
{
    public string Id { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public string Detalle { get; init; } = string.Empty;
    public string Fecha { get; init; } = string.Empty;
    public string Inicio { get; init; } = "—";
    public string Fin { get; init; } = "—";
    public string TiempoJugado { get; init; } = "—";
    public string Total { get; init; } = string.Empty;
    public DateTime FechaOrden { get; init; }
    public int? SesionId { get; init; }
}
