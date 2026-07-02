using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AnexosFolhaFornecedorItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolhaFornecedorItemId",
                table: "Arquivos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arquivos_FolhaFornecedorItemId",
                table: "Arquivos",
                column: "FolhaFornecedorItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arquivos_FolhasFornecedoresItens_FolhaFornecedorItemId",
                table: "Arquivos",
                column: "FolhaFornecedorItemId",
                principalTable: "FolhasFornecedoresItens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arquivos_FolhasFornecedoresItens_FolhaFornecedorItemId",
                table: "Arquivos");

            migrationBuilder.DropIndex(
                name: "IX_Arquivos_FolhaFornecedorItemId",
                table: "Arquivos");

            migrationBuilder.DropColumn(
                name: "FolhaFornecedorItemId",
                table: "Arquivos");
        }
    }
}
