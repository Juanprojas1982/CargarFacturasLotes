using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Enums;

namespace CargarFacturasLotes.Application.UseCases;

public class ProcesarFacturasHandler
{
    private readonly IProcesoFacturaRepository _procesoRepository;
    private readonly IEndpointService _endpointService;

    public ProcesarFacturasHandler(IProcesoFacturaRepository procesoRepository, IEndpointService endpointService)
    {
        _procesoRepository = procesoRepository;
        _endpointService = endpointService;
    }

    public async Task<(int procesados, int exitosos, int errores)> EjecutarAsync(TipoProceso tipoProceso)
    {
        var procesossPendientes = await _procesoRepository.ObtenerPendientesAsync(tipoProceso);
        
        int procesados = 0;
        int exitosos = 0;
        int errores = 0;

        foreach (var proceso in procesossPendientes)
        {
            try
            {
                // Marcar como enviado antes de llamar al endpoint
                proceso.MarcarComoEnviado();
                await _procesoRepository.ActualizarAsync(proceso);
                await _procesoRepository.SaveChangesAsync();

                // Llamar al endpoint correspondiente
                string resultado;
                if (tipoProceso == TipoProceso.Anulacion)
                {
                    resultado = await _endpointService.LlamarEndpointAnulacionAsync(proceso.IdAdmision, proceso.SedeId);
                }
                else
                {
                    resultado = await _endpointService.LlamarEndpointNumeracionAsync(proceso.IdAdmision, proceso.SedeId);
                }

                // Marcar como exitoso
                proceso.MarcarComoExitoso(resultado);
                exitosos++;
            }
            catch (Exception ex)
            {
                // Marcar como error
                proceso.MarcarComoError(ex.Message);
                errores++;
            }

            await _procesoRepository.ActualizarAsync(proceso);
            procesados++;
        }

        await _procesoRepository.SaveChangesAsync();
        return (procesados, exitosos, errores);
    }
}