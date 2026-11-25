using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChronoPlay.Migrations
{
    /// <inheritdoc />
    public partial class AddConsolas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consolas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioHora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    Ocupada = table.Column<bool>(type: "bit", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TiempoJugado = table.Column<TimeSpan>(type: "time", nullable: false),
                    TiempoObjetivo = table.Column<TimeSpan>(type: "time", nullable: false),
                    TiempoLibre = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consolas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consolas");
        }
    }
}
