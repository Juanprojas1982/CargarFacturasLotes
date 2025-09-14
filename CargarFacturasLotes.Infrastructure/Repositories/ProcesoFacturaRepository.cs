using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Entities;
using CargarFacturasLotes.Domain.Enums;
using CargarFacturasLotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CargarFacturasLotes.Infrastructure.Repositories;

public class ProcesoFacturaRepository : IProcesoFacturaRepository
{
    private readonly AppDbContext _context;

    public ProcesoFacturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteProcesoAsync(TipoProceso tipo, string noFactura, int idAdmision, int sedeId, EstadoProceso estado)
    {
        return await _context.ProcesosFacturas
            .AnyAsync(p => p.Tipo == tipo && 
                          p.NoFactura == noFactura && 
                          p.IdAdmision == idAdmision && 
                          p.SedeId == sedeId && 
                          p.Estado == estado);
    }

    public async Task<ProcesoFactura> InsertarAsync(ProcesoFactura proceso)
    {
        await _context.ProcesosFacturas.AddAsync(proceso);
        return proceso;
    }

    public async Task<List<ProcesoFactura>> ObtenerPendientesAsync(TipoProceso tipo)
    {
        return await _context.ProcesosFacturas
            .Where(p => p.Tipo == tipo && p.Estado == EstadoProceso.SinEnviar)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task ActualizarAsync(ProcesoFactura proceso)
    {
        _context.ProcesosFacturas.Update(proceso);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}