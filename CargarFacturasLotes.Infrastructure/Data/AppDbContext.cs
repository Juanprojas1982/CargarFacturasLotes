using CargarFacturasLotes.Domain.Entities;
using CargarFacturasLotes.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargarFacturasLotes.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ProcesoFactura> ProcesosFacturas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcesoFactura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Tipo)
                .HasConversion(
                    v => v.ToString(),
                    v => (TipoProceso)Enum.Parse(typeof(TipoProceso), v))
                .HasMaxLength(20);
                
            entity.Property(e => e.NoFactura)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Estado)
                .HasConversion(
                    v => v.ToString(),
                    v => (EstadoProceso)Enum.Parse(typeof(EstadoProceso), v))
                .HasMaxLength(20);
                
            entity.Property(e => e.Resultado)
                .HasMaxLength(1000);

            // Índice único para evitar duplicados
            entity.HasIndex(e => new { e.Tipo, e.NoFactura, e.IdAdmision, e.SedeId, e.Estado })
                .HasDatabaseName("IX_ProcesosFacturas_Unique");
        });

        base.OnModelCreating(modelBuilder);
    }
}