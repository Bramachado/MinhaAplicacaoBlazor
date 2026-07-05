using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class EnriqueceContaBancaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DigitoConta",
                table: "ContasBancarias",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoChavePix",
                table: "ContasBancarias",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Nenhuma");

            migrationBuilder.AddColumn<string>(
                name: "TipoConta",
                table: "ContasBancarias",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Corrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DigitoConta",
                table: "ContasBancarias");

            migrationBuilder.DropColumn(
                name: "TipoChavePix",
                table: "ContasBancarias");

            migrationBuilder.DropColumn(
                name: "TipoConta",
                table: "ContasBancarias");
        }
    }
}
