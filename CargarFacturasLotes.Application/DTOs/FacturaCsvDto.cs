namespace CargarFacturasLotes.Application.DTOs;

public class FacturaCsvDto
{
    public string NoFactura { get; set; } = string.Empty;
    public int IdAdmision { get; set; }
    public int SedeId { get; set; }
}