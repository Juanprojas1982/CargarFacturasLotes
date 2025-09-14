using CargarFacturasLotes.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace CargarFacturasLotes.Infrastructure.Services;

public class EndpointService : IEndpointService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointAnulacion;
    private readonly string _endpointNumeracion;

    public EndpointService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _endpointAnulacion = configuration["Endpoints:EndpointAnulacion"] 
            ?? throw new ArgumentException("EndpointAnulacion no configurado");
        _endpointNumeracion = configuration["Endpoints:EndpointNumeracion"] 
            ?? throw new ArgumentException("EndpointNumeracion no configurado");
    }

    public async Task<string> LlamarEndpointAnulacionAsync(int idAdmision, int sedeId)
    {
        return await LlamarEndpointAsync(_endpointAnulacion, idAdmision, sedeId);
    }

    public async Task<string> LlamarEndpointNumeracionAsync(int idAdmision, int sedeId)
    {
        return await LlamarEndpointAsync(_endpointNumeracion, idAdmision, sedeId);
    }

    private async Task<string> LlamarEndpointAsync(string endpoint, int idAdmision, int sedeId)
    {
        try
        {
            var requestData = new
            {
                IdAdmision = idAdmision,
                SedeId = sedeId
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                throw new HttpRequestException($"Error HTTP {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al llamar endpoint {endpoint}: {ex.Message}", ex);
        }
    }
}