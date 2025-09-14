using CargarFacturasLotes.Application.Interfaces;

namespace CargarFacturasLotes.Tests.Services;

/// <summary>
/// Fake implementation of IEndpointService that returns "Procesamiento Exitoso" 
/// for nullification operations as required by the test specification.
/// </summary>
public class FakeEndpointService : IEndpointService
{
    public async Task<string> LlamarEndpointAnulacionAsync(int idAdmision, int sedeId)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        // Return the expected success message for nullification
        return "Procesamiento Exitoso";
    }

    public async Task<string> LlamarEndpointNumeracionAsync(int idAdmision, int sedeId)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        // Return success message for numbering operation
        return "Procesamiento Exitoso";
    }
}