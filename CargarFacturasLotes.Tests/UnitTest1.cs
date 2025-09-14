using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Domain.Enums;
using CargarFacturasLotes.Tests.DataGenerators;
using CargarFacturasLotes.Tests.Services;

namespace CargarFacturasLotes.Tests;

/// <summary>
/// Test class that demonstrates fake invoice generation and nullification
/// using a fake endpoint service that returns "Procesamiento Exitoso"
/// </summary>
public class FakeInvoiceProcessingTests
{
    private readonly IEndpointService _fakeEndpointService;

    public FakeInvoiceProcessingTests()
    {
        _fakeEndpointService = new FakeEndpointService();
    }

    [Fact]
    public void GenerateFakeInvoiceList_Should_CreateSpecifiedNumberOfInvoices()
    {
        // Arrange
        var expectedCount = 15;

        // Act
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(expectedCount);

        // Assert
        Assert.NotNull(fakeInvoices);
        Assert.Equal(expectedCount, fakeInvoices.Count);
        Assert.All(fakeInvoices, invoice =>
        {
            Assert.NotNull(invoice.NoFactura);
            Assert.True(invoice.IdAdmision > 0);
            Assert.True(invoice.SedeId > 0);
        });
    }

    [Fact]
    public void GenerateFakeNullificationProcesses_Should_CreateAnulacionProcesses()
    {
        // Arrange
        var expectedCount = 8;

        // Act
        var fakeProcesses = FakeInvoiceGenerator.GenerateFakeNullificationProcesses(expectedCount);

        // Assert
        Assert.NotNull(fakeProcesses);
        Assert.Equal(expectedCount, fakeProcesses.Count);
        Assert.All(fakeProcesses, proceso =>
        {
            Assert.Equal(TipoProceso.Anulacion, proceso.Tipo);
            Assert.Equal(EstadoProceso.SinEnviar, proceso.Estado);
            Assert.NotNull(proceso.NoFactura);
            Assert.True(proceso.IdAdmision > 0);
            Assert.True(proceso.SedeId > 0);
        });
    }

    [Fact]
    public async Task FakeEndpointService_LlamarEndpointAnulacionAsync_Should_ReturnProcesamientoExitoso()
    {
        // Arrange
        var idAdmision = 1234;
        var sedeId = 5;

        // Act
        var result = await _fakeEndpointService.LlamarEndpointAnulacionAsync(idAdmision, sedeId);

        // Assert
        Assert.Equal("Procesamiento Exitoso", result);
    }

    [Fact]
    public async Task ProcessFakeInvoiceNullification_Should_CompleteSuccessfully()
    {
        // Arrange
        var fakeProcess = FakeInvoiceGenerator.GenerateSingleFakeProcess(TipoProceso.Anulacion);

        // Act
        fakeProcess.MarcarComoEnviado();
        var endpointResult = await _fakeEndpointService.LlamarEndpointAnulacionAsync(
            fakeProcess.IdAdmision, 
            fakeProcess.SedeId);
        fakeProcess.MarcarComoExitoso(endpointResult);

        // Assert
        Assert.Equal(EstadoProceso.Exitoso, fakeProcess.Estado);
        Assert.Equal("Procesamiento Exitoso", fakeProcess.Resultado);
    }

    [Fact]
    public async Task ProcessMultipleFakeInvoiceNullifications_Should_AllReturnProcesamientoExitoso()
    {
        // Arrange
        var fakeProcesses = FakeInvoiceGenerator.GenerateFakeNullificationProcesses(10);

        // Act & Assert
        foreach (var proceso in fakeProcesses)
        {
            proceso.MarcarComoEnviado();
            var result = await _fakeEndpointService.LlamarEndpointAnulacionAsync(
                proceso.IdAdmision, 
                proceso.SedeId);
            proceso.MarcarComoExitoso(result);

            Assert.Equal(EstadoProceso.Exitoso, proceso.Estado);
            Assert.Equal("Procesamiento Exitoso", proceso.Resultado);
        }
    }

    [Fact]
    public void DisplayFakeInvoiceList_Should_ShowInvoiceDetails()
    {
        // Arrange
        var fakeInvoices = FakeInvoiceGenerator.GenerateFakeInvoiceList(5);

        // Act - This would typically display the list, here we just verify the data
        var invoiceDetails = fakeInvoices.Select(invoice => 
            $"Factura: {invoice.NoFactura}, Admision: {invoice.IdAdmision}, Sede: {invoice.SedeId}")
            .ToList();

        // Assert
        Assert.Equal(5, invoiceDetails.Count);
        Assert.All(invoiceDetails, detail => 
        {
            Assert.Contains("Factura:", detail);
            Assert.Contains("Admision:", detail);
            Assert.Contains("Sede:", detail);
        });
    }
}