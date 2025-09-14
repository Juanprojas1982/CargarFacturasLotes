using CargarFacturasLotes.Application.DTOs;
using CargarFacturasLotes.Application.Interfaces;

namespace CargarFacturasLotes.Infrastructure.Services;

public class CsvReaderService : ICsvReaderService
{
    public async Task<List<FacturaCsvDto>> LeerCsvAsync(string rutaArchivo)
    {
        var facturas = new List<FacturaCsvDto>();

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException($"El archivo CSV no existe: {rutaArchivo}");
        }

        var lineas = await File.ReadAllLinesAsync(rutaArchivo);
        
        // Saltar la primera línea si es encabezado (opcional)
        bool tieneEncabezado = lineas.Length > 0 && !int.TryParse(lineas[0].Split(',')[1], out _);
        int inicioLineas = tieneEncabezado ? 1 : 0;

        for (int i = inicioLineas; i < lineas.Length; i++)
        {
            var linea = lineas[i].Trim();
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            var campos = linea.Split(',');
            
            if (campos.Length < 3)
            {
                throw new FormatException($"Línea {i + 1}: Formato inválido. Se esperan 3 campos separados por coma.");
            }

            try
            {
                var factura = new FacturaCsvDto
                {
                    NoFactura = campos[0].Trim().Trim('"'),
                    IdAdmision = int.Parse(campos[1].Trim()),
                    SedeId = int.Parse(campos[2].Trim())
                };

                facturas.Add(factura);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Línea {i + 1}: Error al parsear datos - {ex.Message}");
            }
        }

        return facturas;
    }
}