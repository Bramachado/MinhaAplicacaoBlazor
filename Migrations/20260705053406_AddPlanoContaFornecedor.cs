using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanoContaFornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanoContaId",
                table: "Fornecedores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_PlanoContaId",
                table: "Fornecedores",
                column: "PlanoContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedores_PlanosContas_PlanoContaId",
                table: "Fornecedores",
                column: "PlanoContaId",
                principalTable: "PlanosContas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedores_PlanosContas_PlanoContaId",
                table: "Fornecedores");

            migrationBuilder.DropIndex(
                name: "IX_Fornecedores_PlanoContaId",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "PlanoContaId",
                table: "Fornecedores");
        }
    }
}
