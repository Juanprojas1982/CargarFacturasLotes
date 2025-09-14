using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Enums;
using CargarFacturasLotes.Tests.DataGenerators;
using CargarFacturasLotes.Tests.Services;

namespace CargarFacturasLotes.Tests.Demos;

/// <summary>
/// Demo class that shows how to use the fake invoice generator and endpoint service
/// This demonstrates the requirements from the issue: fake invoice list and fake URL consumption
/// </summary>
public class FakeInvoiceDemo
{
    private readonly IEndpointService _fakeEndpointService;

    public FakeInvoiceDemo()
    {
        _fakeEndpointService = new FakeEndpointService();
    }

    /// <summary>
    /// Demonstrates generating a fake invoice list and processing nullifications
    /// </summary>
    public async Task RunDemoAsync()
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("DEMO: Sistema de Pruebas con Listado Fake de Facturas y URLs Fake");
        Console.WriteLine("================================================================================");

        // 1. Generate fake invoice list (as requested in the issue)
        Console.WriteLine("\n1. Generando Listado Fake de Facturas:");
        Console.WriteLine("----------------------------------------");
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(10);
        
        foreach (var (invoice, index) in fakeInvoices.Select((inv, i) => (inv, i + 1)))
        {
            Console.WriteLine($"   {index:D2}. Factura: {invoice.NoFactura} | Admisión: {invoice.IdAdmision} | Sede: {invoice.SedeId}");
        }

        Console.WriteLine($"\nTotal de facturas generadas: {fakeInvoices.Count}");

        // 2. Process nullifications using fake URL (as requested in the issue)
        Console.WriteLine("\n2. Procesando Anulaciones con URL Fake:");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("El webservice de anulación devuelve: \"Procesamiento Exitoso\"");
        Console.WriteLine();

        var processedCount = 0;
        var successCount = 0;

        foreach (var invoice in fakeInvoices)
        {
            processedCount++;
            
            // Create nullification process
            var proceso = FakeInvoiceGenerator.GenerateSingleFakeProcess(
                TipoProceso.Anulacion,
                invoice.NoFactura,
                invoice.IdAdmision,
                invoice.SedeId);

            try
            {
                // Mark as sent
                proceso.MarcarComoEnviado();
                
                // Call fake URL for nullification
                var result = await _fakeEndpointService.LlamarEndpointAnulacionAsync(
                    proceso.IdAdmision, 
                    proceso.SedeId);
                
                // Mark as successful
                proceso.MarcarComoExitoso(result);
                successCount++;
                
                Console.WriteLine($"   ✓ Factura {proceso.NoFactura}: {result}");
            }
            catch (Exception ex)
            {
                proceso.MarcarComoError(ex.Message);
                Console.WriteLine($"   ✗ Factura {proceso.NoFactura}: Error - {ex.Message}");
            }
        }

        // 3. Show summary
        Console.WriteLine("\n3. Resumen del Procesamiento:");
        Console.WriteLine("------------------------------");
        Console.WriteLine($"   Total procesadas: {processedCount}");
        Console.WriteLine($"   Exitosas: {successCount}");
        Console.WriteLine($"   Errores: {processedCount - successCount}");
        Console.WriteLine($"   Tasa de éxito: {(double)successCount / processedCount * 100:F1}%");

        Console.WriteLine("\n================================================================================");
        Console.WriteLine("DEMO COMPLETADO EXITOSAMENTE");
        Console.WriteLine("================================================================================");
    }

    /// <summary>
    /// Shows different types of fake data generation
    /// </summary>
    public void ShowDataGenerationCapabilities()
    {
        Console.WriteLine("\n=== Capacidades del Generador de Datos Fake ===");
        
        // Generate different sizes of invoice lists
        var smallList = FakeInvoiceGenerator.GenerateFakeInvoiceList(3);
        var mediumList = FakeInvoiceGenerator.GenerateFakeInvoiceList(10);
        var largeList = FakeInvoiceGenerator.GenerateFakeInvoiceList(50);
        
        Console.WriteLine($"Lista pequeña: {smallList.Count} facturas");
        Console.WriteLine($"Lista mediana: {mediumList.Count} facturas");
        Console.WriteLine($"Lista grande: {largeList.Count} facturas");
        
        // Generate nullification processes
        var nullificationProcesses = FakeInvoiceGenerator.GenerateFakeNullificationProcesses(5);
        Console.WriteLine($"Procesos de anulación: {nullificationProcesses.Count} procesos");
        
        // Generate custom invoice
        var customInvoice = FakeInvoiceGenerator.GenerateSingleFakeInvoice("CUSTOM001", 12345, 99);
        Console.WriteLine($"Factura personalizada: {customInvoice.NoFactura} - {customInvoice.IdAdmision} - {customInvoice.SedeId}");
    }

    /// <summary>
    /// Demonstrates fake URL responses
    /// </summary>
    public async Task ShowFakeUrlResponsesAsync()
    {
        Console.WriteLine("\n=== Respuestas de URLs Fake ===");
        
        // Test nullification endpoint
        var anulacionResult = await _fakeEndpointService.LlamarEndpointAnulacionAsync(12345, 1);
        Console.WriteLine($"Endpoint Anulación: {anulacionResult}");
        
        // Test numbering endpoint
        var numeracionResult = await _fakeEndpointService.LlamarEndpointNumeracionAsync(67890, 2);
        Console.WriteLine($"Endpoint Numeración: {numeracionResult}");
        
        Console.WriteLine("Ambos endpoints siempre devuelven: \"Procesamiento Exitoso\"");
    }
}