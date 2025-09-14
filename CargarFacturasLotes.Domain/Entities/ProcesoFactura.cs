using CargarFacturasLotes.Domain.Enums;

namespace CargarFacturasLotes.Domain.Entities;

public class ProcesoFactura
{
    public int Id { get; set; }
    public TipoProceso Tipo { get; set; }
    public string NoFactura { get; set; } = string.Empty;
    public int IdAdmision { get; set; }
    public int SedeId { get; set; }
    public EstadoProceso Estado { get; set; }
    public string? Resultado { get; set; }
    
    // Constructor parameterless para EF Core
    public ProcesoFactura() { }
    
    // Constructor para crear nueva instancia
    public ProcesoFactura(TipoProceso tipo, string noFactura, int idAdmision, int sedeId)
    {
        Tipo = tipo;
        NoFactura = noFactura;
        IdAdmision = idAdmision;
        SedeId = sedeId;
        Estado = EstadoProceso.SinEnviar;
    }
    
    public void MarcarComoEnviado()
    {
        Estado = EstadoProceso.Enviado;
    }
    
    public void MarcarComoExitoso(string resultado)
    {
        Estado = EstadoProceso.Exitoso;
        Resultado = resultado;
    }
    
    public void MarcarComoError(string mensajeError)
    {
        Estado = EstadoProceso.Error;
        Resultado = mensajeError;
    }
}