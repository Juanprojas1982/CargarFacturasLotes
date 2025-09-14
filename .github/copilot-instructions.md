# Instrucciones para GitHub Copilot en este repositorio

## Tipo de proyecto
- Solución Blank en Visual Studio 2022.
- Arquitectura basada en Clean Architecture (Manual2024).

## Estructura de proyectos
- **CargarFacturasLotes.Domain**
  - Entidades, Enumeraciones, Excepciones, Agregados, Eventos de dominio.
- **CargarFacturasLotes.Application**
  - Casos de uso (Interactors), DTOs, interfaces de puertos de entrada/salida.
- **CargarFacturasLotes.Infrastructure**
  - Repositorios, servicios externos, logging, acceso a datos.
- **CargarFacturasLotes.ConsoleApp**
  - Punto de entrada. Consume la capa Application vía interfaces.

## Reglas de dependencia
- ConsoleApp → Application → Domain  
- Infrastructure → Application, Domain  
- Nunca invertir las dependencias.

## Buenas prácticas
- Usar Inyección de dependencias.
- Aplicar CQRS, Repository y Unit of Work.
- Validación desacoplada (FluentValidation).
- Manejo centralizado de excepciones y logging.

## Expectativas de código
- Implementar DbContext y repositorios con EF Core para SQL Server.
- Usar `System.Net.Http.HttpClient` para consumir endpoints configurados en `appsettings.json`.
- Procesar CSVs con separador coma, validando duplicados antes de insertar en BD.
- Registrar estado de procesamiento en tabla `ProcesosFacturas` con control de reintentos y reanudación.
