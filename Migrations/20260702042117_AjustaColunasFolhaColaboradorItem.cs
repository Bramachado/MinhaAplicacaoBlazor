using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AjustaColunasFolhaColaboradorItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O antigo "Salário Bruto" passa a ser o "Salário Base" (mantém os valores).
            migrationBuilder.RenameColumn(
                name: "SalarioBruto",
                table: "FolhasColaboradoresItens",
                newName: "SalarioBase");

            migrationBuilder.DropColumn(
                name: "SalarioLiquidoBase",
                table: "FolhasColaboradoresItens");

            migrationBuilder.AddColumn<decimal>(
                name: "OutrosProventos",
                table: "FolhasColaboradoresItens",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutrosProventos",
                table: "FolhasColaboradoresItens");

            migrationBuilder.RenameColumn(
                name: "SalarioBase",
                table: "FolhasColaboradoresItens",
                newName: "SalarioBruto");

            migrationBuilder.AddColumn<decimal>(
                name: "SalarioLiquidoBase",
                table: "FolhasColaboradoresItens",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
