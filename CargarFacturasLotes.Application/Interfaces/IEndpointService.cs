namespace CargarFacturasLotes.Application.Interfaces;

public interface IEndpointService
{
    Task<string> LlamarEndpointNumeracionAsync(int idAdmision, int sedeId);
}