using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoPessoaFormaContaBancaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Forma",
                table: "ContasBancarias",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "PIX");

            migrationBuilder.AddColumn<string>(
                name: "TipoPessoa",
                table: "ContasBancarias",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Fisica");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Forma",
                table: "ContasBancarias");

            migrationBuilder.DropColumn(
                name: "TipoPessoa",
                table: "ContasBancarias");
        }
    }
}
