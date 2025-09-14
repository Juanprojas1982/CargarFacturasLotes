namespace CargarFacturasLotes.Application.Interfaces;

public interface IEndpointService
{
    Task<string> LlamarEndpointAnulacionAsync(int idAdmision, int sedeId);
    Task<string> LlamarEndpointNumeracionAsync(int idAdmision, int sedeId);
}