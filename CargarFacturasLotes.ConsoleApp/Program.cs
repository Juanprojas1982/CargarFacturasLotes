using CargarFacturasLotes.Application.Interfaces;
using CargarFacturasLotes.Application.UseCases;
using CargarFacturasLotes.Domain.Enums;
using CargarFacturasLotes.Infrastructure.Data;
using CargarFacturasLotes.Infrastructure.Repositories;
using CargarFacturasLotes.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CargarFacturasLotes.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            // Asegurar que la base de datos existe
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            // Ejecutar la aplicación
            var app = services.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurrió un error durante la ejecución");
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Presiona cualquier tecla para salir...");
        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Configuración de Entity Framework
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                // Repositorios
                services.AddScoped<IProcesoFacturaRepository, ProcesoFacturaRepository>();

                // Servicios
                services.AddScoped<ICsvReaderService, CsvReaderService>();
                services.AddHttpClient<IEndpointService, EndpointService>();

                // Casos de uso
                services.AddScoped<RegistrarFacturasHandler>();
                services.AddScoped<ProcesarFacturasHandler>();

                // Aplicación principal
                services.AddScoped<ConsoleApplication>();
            });
}
