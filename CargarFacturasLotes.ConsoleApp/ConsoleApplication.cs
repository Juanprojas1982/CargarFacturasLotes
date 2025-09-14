using CargarFacturasLotes.Application.UseCases;
using CargarFacturasLotes.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CargarFacturasLotes.ConsoleApp;

public class ConsoleApplication
{
    private readonly RegistrarFacturasHandler _registrarFacturasHandler;
    private readonly ProcesarFacturasHandler _procesarFacturasHandler;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConsoleApplication> _logger;

    public ConsoleApplication(
        RegistrarFacturasHandler registrarFacturasHandler,
        ProcesarFacturasHandler procesarFacturasHandler,
        IConfiguration configuration,
        ILogger<ConsoleApplication> logger)
    {
        _registrarFacturasHandler = registrarFacturasHandler;
        _procesarFacturasHandler = procesarFacturasHandler;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("=== Sistema de Carga de Facturas en Lotes ===");
        Console.WriteLine();

        while (true)
        {
            MostrarMenu();
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await ProcesarAnulacionAsync();
                    break;
                case "2":
                    await ProcesarNumeracionAsync();
                    break;
                case "3":
                    Console.WriteLine("Saliendo del sistema...");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("Seleccione una opción:");
        Console.WriteLine("1. Anular facturas");
        Console.WriteLine("2. Renumerar facturas");
        Console.WriteLine("3. Salir");
        Console.Write("Opción: ");
    }

    private async Task ProcesarAnulacionAsync()
    {
        Console.WriteLine("=== ANULACIÓN DE FACTURAS ===");
        await ProcesarTipoFacturaAsync(TipoProceso.Anulacion);
    }

    private async Task ProcesarNumeracionAsync()
    {
        Console.WriteLine("=== RENUMERACIÓN DE FACTURAS ===");
        await ProcesarTipoFacturaAsync(TipoProceso.Numeracion);
    }

    private async Task ProcesarTipoFacturaAsync(TipoProceso tipoProceso)
    {
        try
        {
            var rutaCsv = _configuration["CsvPath"];
            if (string.IsNullOrEmpty(rutaCsv))
            {
                Console.WriteLine("Error: No se ha configurado la ruta del archivo CSV (CsvPath)");
                return;
            }

            Console.WriteLine($"Ruta del archivo CSV: {rutaCsv}");

            // Paso 1: Registrar facturas desde CSV
            Console.WriteLine("Paso 1: Leyendo y registrando facturas desde CSV...");
            
            var registrosInsertados = await _registrarFacturasHandler.EjecutarAsync(rutaCsv, tipoProceso);
            Console.WriteLine($"Registros insertados: {registrosInsertados}");

            if (registrosInsertados == 0)
            {
                Console.WriteLine("No hay nuevos registros para procesar.");
                return;
            }

            // Paso 2: Procesar facturas pendientes
            Console.WriteLine("Paso 2: Procesando facturas pendientes...");
            
            var (procesados, exitosos, errores) = await _procesarFacturasHandler.EjecutarAsync(tipoProceso);
            
            Console.WriteLine($"Resumen de procesamiento:");
            Console.WriteLine($"- Total procesados: {procesados}");
            Console.WriteLine($"- Exitosos: {exitosos}");
            Console.WriteLine($"- Errores: {errores}");

            if (errores > 0)
            {
                Console.WriteLine("Nota: Los registros con error permanecen en la base de datos para revisión.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el procesamiento de facturas");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}