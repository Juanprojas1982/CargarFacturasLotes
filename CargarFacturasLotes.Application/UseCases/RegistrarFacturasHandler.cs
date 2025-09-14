using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Entities;
using CargarFacturasLotes.Domain.Enums;

namespace CargarFacturasLotes.Application.UseCases;

public class RegistrarFacturasHandler
{
    private readonly ICsvReaderService _csvReaderService;
    private readonly IProcesoFacturaRepository _procesoRepository;

    public RegistrarFacturasHandler(ICsvReaderService csvReaderService, IProcesoFacturaRepository procesoRepository)
    {
        _csvReaderService = csvReaderService;
        _procesoRepository = procesoRepository;
    }

    public async Task<int> EjecutarAsync(string rutaCsv, TipoProceso tipoProceso)
    {
        var facturas = await _csvReaderService.LeerCsvAsync(rutaCsv);
        var registrosInsertados = 0;

        foreach (var factura in facturas)
        {
            // Verificar si ya existe un registro con la misma combinación
            var existe = await _procesoRepository.ExisteProcesoAsync(
                tipoProceso, 
                factura.NoFactura, 
                factura.IdAdmision, 
                factura.SedeId, 
                EstadoProceso.SinEnviar
            );

            if (!existe)
            {
                var procesoFactura = new ProcesoFactura(
                    tipoProceso,
                    factura.NoFactura,
                    factura.IdAdmision,
                    factura.SedeId
                );

                await _procesoRepository.InsertarAsync(procesoFactura);
                registrosInsertados++;
            }
        }

        await _procesoRepository.SaveChangesAsync();
        return registrosInsertados;
    }
}