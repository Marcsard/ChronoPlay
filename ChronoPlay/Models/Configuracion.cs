public class Configuracion
{
    public int Id { get; set; }

    public string NombreNegocio { get; set; } = "";

    public string TiempoPredeterminado { get; set; } = "";

    public string ModoCobro { get; set; } = "";

    public bool SonidoAlarma { get; set; }

    public string RetencionHistorial { get; set; } = "";

    public bool RespaldosAutomaticos { get; set; }
}