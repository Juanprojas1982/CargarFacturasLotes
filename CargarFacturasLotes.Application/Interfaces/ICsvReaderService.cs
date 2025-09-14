using CargarFacturasLotes.Application.DTOs;

namespace CargarFacturasLotes.Application.Interfaces;

public interface ICsvReaderService
{
    Task<List<FacturaCsvDto>> LeerCsvAsync(string rutaArchivo);
}