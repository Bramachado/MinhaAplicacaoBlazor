using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddCargoSalarioColaborador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CargaHorariaSemanal",
                table: "Colaboradores",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "Colaboradores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalarioBase",
                table: "Colaboradores",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TicketAlimentacao",
                table: "Colaboradores",
                type: "decimal(12,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargaHorariaSemanal",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "SalarioBase",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "TicketAlimentacao",
                table: "Colaboradores");
        }
    }
}
