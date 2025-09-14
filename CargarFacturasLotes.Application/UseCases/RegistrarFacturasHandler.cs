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
            // Verificar si ya existe un registro con la misma combinación (sin importar el estado)
            // Esto evita recargar facturas que ya fueron procesadas exitosamente
            var existeExitoso = await _procesoRepository.ExisteProcesoAsync(
                tipoProceso, 
                factura.NoFactura, 
                factura.IdAdmision, 
                factura.SedeId, 
                EstadoProceso.Exitoso
            );
            
            var existePendiente = await _procesoRepository.ExisteProcesoAsync(
                tipoProceso, 
                factura.NoFactura, 
                factura.IdAdmision, 
                factura.SedeId, 
                EstadoProceso.SinEnviar
            );

            if (!existeExitoso && !existePendiente)
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