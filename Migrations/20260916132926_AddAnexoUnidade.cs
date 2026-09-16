using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddAnexoUnidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadeId",
                table: "Arquivos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arquivos_UnidadeId",
                table: "Arquivos",
                column: "UnidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arquivos_Unidades_UnidadeId",
                table: "Arquivos",
                column: "UnidadeId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arquivos_Unidades_UnidadeId",
                table: "Arquivos");

            migrationBuilder.DropIndex(
                name: "IX_Arquivos_UnidadeId",
                table: "Arquivos");

            migrationBuilder.DropColumn(
                name: "UnidadeId",
                table: "Arquivos");
        }
    }
}
