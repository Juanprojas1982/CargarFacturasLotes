using CargarFacturasLotes.Domain.Entities;
using CargarFacturasLotes.Domain.Enums;

namespace CargarFacturasLotes.Application.Interfaces;

public interface IProcesoFacturaRepository
{
    Task<bool> ExisteProcesoAsync(TipoProceso tipo, string noFactura, int idAdmision, int sedeId, EstadoProceso estado);
    Task<ProcesoFactura> InsertarAsync(ProcesoFactura proceso);
    Task<List<ProcesoFactura>> ObtenerPendientesAsync(TipoProceso tipo);
    Task ActualizarAsync(ProcesoFactura proceso);
    Task<int> SaveChangesAsync();
}