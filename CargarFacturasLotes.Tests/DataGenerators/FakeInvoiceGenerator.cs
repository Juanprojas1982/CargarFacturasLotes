using CargarFacturasLotes.Application.DTOs;
using CargarFacturasLotes.Domain.Entities;
using CargarFacturasLotes.Domain.Enums;

namespace CargarFacturasLotes.Tests.DataGenerators;

/// <summary>
/// Generates fake invoice data for testing purposes
/// </summary>
public static class FakeInvoiceGenerator
{
    private static readonly Random _random = new();

    /// <summary>
    /// Generates a list of fake FacturaCsvDto objects
    /// </summary>
    /// <param name="count">Number of fake invoices to generate</param>
    /// <returns>List of fake invoices</returns>
    public static List<FacturaCsvDto> GenerateFakeInvoiceList(int count = 10)
    {
        var fakeInvoices = new List<FacturaCsvDto>();
        
        for (int i = 1; i <= count; i++)
        {
            fakeInvoices.Add(new FacturaCsvDto
            {
                NoFactura = $"FAC{DateTime.Now:yyyyMMdd}{i:D4}",
                IdAdmision = _random.Next(1000, 9999),
                SedeId = _random.Next(1, 10)
            });
        }
        
        return fakeInvoices;
    }

    /// <summary>
    /// Generates a list of fake ProcesoFactura entities for nullification testing
    /// </summary>
    /// <param name="count">Number of fake processes to generate</param>
    /// <returns>List of fake invoice processes</returns>
    public static List<ProcesoFactura> GenerateFakeNullificationProcesses(int count = 5)
    {
        var fakeProcesses = new List<ProcesoFactura>();
        
        for (int i = 1; i <= count; i++)
        {
            var proceso = new ProcesoFactura(
                TipoProceso.Anulacion,
                $"FAC{DateTime.Now:yyyyMMdd}{i:D4}",
                _random.Next(1000, 9999),
                _random.Next(1, 10)
            );
            
            fakeProcesses.Add(proceso);
        }
        
        return fakeProcesses;
    }

    /// <summary>
    /// Generates a single fake invoice with specific parameters
    /// </summary>
    /// <param name="noFactura">Invoice number (optional)</param>
    /// <param name="idAdmision">Admission ID (optional)</param>
    /// <param name="sedeId">Sede ID (optional)</param>
    /// <returns>Single fake invoice</returns>
    public static FacturaCsvDto GenerateSingleFakeInvoice(
        string? noFactura = null, 
        int? idAdmision = null, 
        int? sedeId = null)
    {
        return new FacturaCsvDto
        {
            NoFactura = noFactura ?? $"FAC{DateTime.Now:yyyyMMddHHmmss}",
            IdAdmision = idAdmision ?? _random.Next(1000, 9999),
            SedeId = sedeId ?? _random.Next(1, 10)
        };
    }

    /// <summary>
    /// Generates a single fake ProcesoFactura for testing
    /// </summary>
    /// <param name="tipo">Process type</param>
    /// <param name="noFactura">Invoice number (optional)</param>
    /// <param name="idAdmision">Admission ID (optional)</param>
    /// <param name="sedeId">Sede ID (optional)</param>
    /// <returns>Single fake process</returns>
    public static ProcesoFactura GenerateSingleFakeProcess(
        TipoProceso tipo = TipoProceso.Anulacion,
        string? noFactura = null,
        int? idAdmision = null,
        int? sedeId = null)
    {
        return new ProcesoFactura(
            tipo,
            noFactura ?? $"FAC{DateTime.Now:yyyyMMddHHmmss}",
            idAdmision ?? _random.Next(1000, 9999),
            sedeId ?? _random.Next(1, 10)
        );
    }
}