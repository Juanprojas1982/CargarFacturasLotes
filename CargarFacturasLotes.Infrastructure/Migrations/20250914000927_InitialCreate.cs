using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargarFacturasLotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcesosFacturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NoFactura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdAdmision = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesosFacturas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcesosFacturas_Unique",
                table: "ProcesosFacturas",
                columns: new[] { "Tipo", "NoFactura", "IdAdmision", "SedeId", "Estado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcesosFacturas");
        }
    }
}
