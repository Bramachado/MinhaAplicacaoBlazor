using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddParcelamentoFolhaFornecedorItemNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroParcela",
                table: "FolhasFornecedoresItensNotas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TotalParcelas",
                table: "FolhasFornecedoresItensNotas",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroParcela",
                table: "FolhasFornecedoresItensNotas");

            migrationBuilder.DropColumn(
                name: "TotalParcelas",
                table: "FolhasFornecedoresItensNotas");
        }
    }
}
