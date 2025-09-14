# CargarFacturasLotes - Sistema de Carga de Facturas en Lotes

Una aplicación de consola .NET 8 que implementa Clean Architecture para procesar facturas en lotes mediante CSV y llamadas a endpoints HTTP.

## Arquitectura

El proyecto sigue los principios de Clean Architecture con cuatro capas:

- **CargarFacturasLotes.Domain**: Entidades, enumeraciones y lógica de dominio
- **CargarFacturasLotes.Application**: Casos de uso e interfaces de puertos
- **CargarFacturasLotes.Infrastructure**: Implementaciones concretas (EF Core, HTTP clients, CSV readers)
- **CargarFacturasLotes.ConsoleApp**: Punto de entrada de la aplicación

## Características

- ✅ Procesamiento de archivos CSV con validación
- ✅ Prevención de duplicados mediante validación en base de datos
- ✅ Dos tipos de operaciones: Anulación y Numeración de facturas
- ✅ Consumo de endpoints HTTP configurables
- ✅ Reinicio de procesamiento desde registros pendientes
- ✅ Manejo de errores y logging
- ✅ Patrón Repository e Inyección de Dependencias

## Configuración

### Base de Datos

1. Instalar SQL Server
2. Actualizar la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu-servidor;Database=CargarFacturasLotes;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. Ejecutar migraciones:

```bash
cd CargarFacturasLotes.Infrastructure
dotnet ef database update
```

### Endpoints y CSV

Configurar en `appsettings.json`:

```json
{
  "Endpoints": {
    "EndpointAnulacion": "https://tu-api.com/anular",
    "EndpointNumeracion": "https://tu-api.com/numerar"
  },
  "CsvPath": "C:\\ruta\\a\\tu\\archivo.csv"
}
```

### Formato del CSV

El archivo CSV debe tener las siguientes columnas (separadas por coma):

```csv
NoFactura,IdAdmision,SedeId
FAC001,12345,1
FAC002,12346,2
```

## Uso

1. **Compilar el proyecto:**

```bash
dotnet build
```

2. **Ejecutar la aplicación:**

```bash
cd CargarFacturasLotes.ConsoleApp
dotnet run
```

3. **Seleccionar operación:**
   - Opción 1: Anular facturas
   - Opción 2: Renumerar facturas
   - Opción 3: Salir

## Flujo de Procesamiento

1. **Carga inicial**: Lee el CSV y registra las facturas en la tabla `ProcesosFacturas` con estado `SinEnviar`
2. **Validación de duplicados**: Evita recargar facturas ya procesadas exitosamente
3. **Procesamiento**: Para cada registro pendiente:
   - Marca como `Enviado`
   - Llama al endpoint correspondiente
   - Actualiza estado a `Exitoso` o `Error` según la respuesta
4. **Reinicio**: Si la aplicación se interrumpe, al reiniciar continúa desde los registros `SinEnviar`

## Estructura de Base de Datos

### Tabla ProcesosFacturas

| Columna | Tipo | Descripción |
|---------|------|-------------|
| Id | int (PK, Identity) | Identificador único |
| Tipo | string | 'Anulacion' o 'Numeracion' |
| NoFactura | string | Número de factura |
| IdAdmision | int | ID de admisión |
| SedeId | int | ID de sede |
| Estado | string | 'SinEnviar', 'Enviado', 'Exitoso', 'Error' |
| Resultado | string | Respuesta del endpoint o mensaje de error |

## Desarrollo

### Requisitos

- .NET 8 SDK
- SQL Server (o SQL Server Express)
- Visual Studio 2022 o VS Code

### Comandos útiles

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar tests (si existen)
dotnet test

# Agregar migración
dotnet ef migrations add NombreMigracion --project CargarFacturasLotes.Infrastructure

# Actualizar base de datos
dotnet ef database update --project CargarFacturasLotes.Infrastructure
```

### Para desarrollo con base de datos en memoria

Descomentar la línea correspondiente en `Program.cs`:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CargarFacturasLotesDemo"));
```

## Manejo de Errores

- Los errores de HTTP se capturan y almacenan en la columna `Resultado`
- Los registros con error permanecen en la base de datos para revisión manual
- La aplicación continúa procesando otros registros aunque algunos fallen
- Logging completo disponible en la consola

## Extensibilidad

- Agregar nuevos tipos de proceso modificando el enum `TipoProceso`
- Implementar nuevos formatos de entrada creando implementaciones de `ICsvReaderService`
- Agregar validaciones adicionales en la capa Domain
- Implementar nuevos endpoints modificando `IEndpointService`