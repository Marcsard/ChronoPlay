using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChronoPlay.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionV10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreNegocio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TiempoPredeterminado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModoCobro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SonidoAlarma = table.Column<bool>(type: "bit", nullable: false),
                    RetencionHistorial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespaldosAutomaticos = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuraciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuraciones");
        }
    }
}
