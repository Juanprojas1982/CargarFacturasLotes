using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Enums;
using CargarFacturasLotes.Tests.DataGenerators;
using CargarFacturasLotes.Tests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CargarFacturasLotes.Tests.IntegrationTests;

/// <summary>
/// Integration tests demonstrating the complete fake invoice workflow
/// including list generation, fake URL consumption, and success responses
/// </summary>
public class FakeInvoiceWorkflowTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public FakeInvoiceWorkflowTests()
    {
        var services = new ServiceCollection();
        
        // Setup configuration
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: true);
        _configuration = configurationBuilder.Build();
        
        services.AddSingleton(_configuration);
        services.AddSingleton<IEndpointService, FakeEndpointService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task CompleteWorkflow_GenerateFakeInvoicesAndProcessNullification_Should_ReturnProcesamientoExitoso()
    {
        // Arrange - Generate fake invoice list
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(20);
        var endpointService = _serviceProvider.GetRequiredService<IEndpointService>();
        
        // Convert invoices to nullification processes
        var nullificationProcesses = fakeInvoices.Select(invoice =>
            FakeInvoiceGenerator.GenerateSingleFakeProcess(
                TipoProceso.Anulacion,
                invoice.NoFactura,
                invoice.IdAdmision,
                invoice.SedeId)
        ).ToList();

        // Act - Process each nullification
        var results = new List<(string InvoiceNumber, string Result)>();
        
        foreach (var proceso in nullificationProcesses)
        {
            // Mark as sent
            proceso.MarcarComoEnviado();
            
            // Call fake URL for nullification
            var result = await endpointService.LlamarEndpointAnulacionAsync(
                proceso.IdAdmision, 
                proceso.SedeId);
            
            // Mark as successful
            proceso.MarcarComoExitoso(result);
            
            results.Add((proceso.NoFactura, proceso.Resultado!));
        }

        // Assert
        Assert.Equal(20, results.Count);
        Assert.All(results, result =>
        {
            Assert.NotNull(result.InvoiceNumber);
            Assert.Equal("Procesamiento Exitoso", result.Result);
        });
        
        Assert.All(nullificationProcesses, proceso =>
        {
            Assert.Equal(EstadoProceso.Exitoso, proceso.Estado);
            Assert.Equal("Procesamiento Exitoso", proceso.Resultado);
        });
    }

    [Fact]
    public void FakeInvoiceList_Should_DisplayCorrectly()
    {
        // Arrange
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(10);

        // Act - Simulate displaying the fake invoice list
        var displayList = fakeInvoices.Select((invoice, index) => new
        {
            Index = index + 1,
            Invoice = invoice.NoFactura,
            Admission = invoice.IdAdmision,
            Sede = invoice.SedeId,
            Status = "Ready for Processing"
        }).ToList();

        // Assert
        Assert.Equal(10, displayList.Count);
        Assert.All(displayList, item =>
        {
            Assert.True(item.Index > 0);
            Assert.NotNull(item.Invoice);
            Assert.True(item.Admission > 0);
            Assert.True(item.Sede > 0);
            Assert.Equal("Ready for Processing", item.Status);
        });

        // Verify the fake list contains expected data patterns
        Assert.All(fakeInvoices, invoice =>
        {
            Assert.StartsWith("FAC", invoice.NoFactura);
            Assert.InRange(invoice.IdAdmision, 1000, 9999);
            Assert.InRange(invoice.SedeId, 1, 10);
        });
    }

    [Fact]
    public async Task FakeUrlConsumption_ForNullification_Should_AlwaysReturnSuccessMessage()
    {
        // Arrange
        var endpointService = _serviceProvider.GetRequiredService<IEndpointService>();
        var testCases = new[]
        {
            new { IdAdmision = 1001, SedeId = 1 },
            new { IdAdmision = 2002, SedeId = 2 },
            new { IdAdmision = 3003, SedeId = 3 },
            new { IdAdmision = 4004, SedeId = 4 },
            new { IdAdmision = 5005, SedeId = 5 }
        };

        // Act & Assert
        foreach (var testCase in testCases)
        {
            var result = await endpointService.LlamarEndpointAnulacionAsync(
                testCase.IdAdmision, 
                testCase.SedeId);

            Assert.Equal("Procesamiento Exitoso", result);
        }
    }

    [Fact]
    public void Configuration_Should_ContainFakeEndpoints()
    {
        // Act
        var anulacionEndpoint = _configuration["Endpoints:EndpointAnulacion"];
        var numeracionEndpoint = _configuration["Endpoints:EndpointNumeracion"];

        // Assert
        Assert.Equal("https://fake-api.test/anular", anulacionEndpoint);
        Assert.Equal("https://fake-api.test/numerar", numeracionEndpoint);
    }

    [Fact]
    public async Task SimulateConsoleApp_ProcessingFakeInvoices_Should_ShowCompleteWorkflow()
    {
        // Arrange - Simulate console app behavior
        var endpointService = _serviceProvider.GetRequiredService<IEndpointService>();
        
        // Generate fake invoice list (as requested in the issue)
        Console.WriteLine("=== Generando Listado Fake de Facturas ===");
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(5);
        
        foreach (var invoice in fakeInvoices)
        {
            Console.WriteLine($"Factura: {invoice.NoFactura}, Admisión: {invoice.IdAdmision}, Sede: {invoice.SedeId}");
        }

        // Process nullifications using fake URL
        Console.WriteLine("\n=== Procesando Anulaciones con URL Fake ===");
        var processResults = new List<string>();
        
        foreach (var invoice in fakeInvoices)
        {
            // Act - Call fake URL for nullification
            var result = await endpointService.LlamarEndpointAnulacionAsync(
                invoice.IdAdmision, 
                invoice.SedeId);
            
            processResults.Add(result);
            Console.WriteLine($"Factura {invoice.NoFactura}: {result}");
        }

        // Assert
        Assert.Equal(5, fakeInvoices.Count);
        Assert.All(processResults, result => 
            Assert.Equal("Procesamiento Exitoso", result));
        
        Console.WriteLine("\n=== Procesamiento Completado Exitosamente ===");
    }
}