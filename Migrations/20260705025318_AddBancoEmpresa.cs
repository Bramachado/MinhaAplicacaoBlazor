using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddBancoEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BancoPagadorId",
                table: "FolhasFornecedoresItens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoPagamento",
                table: "FolhasFornecedoresItens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BancoId",
                table: "Entradas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeBanco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItens_BancoPagadorId",
                table: "FolhasFornecedoresItens",
                column: "BancoPagadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_BancoId",
                table: "Entradas",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bancos_NomeBanco",
                table: "Bancos",
                column: "NomeBanco",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Bancos_BancoId",
                table: "Entradas",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasFornecedoresItens_Bancos_BancoPagadorId",
                table: "FolhasFornecedoresItens",
                column: "BancoPagadorId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Bancos_BancoId",
                table: "Entradas");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasFornecedoresItens_Bancos_BancoPagadorId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropIndex(
                name: "IX_FolhasFornecedoresItens_BancoPagadorId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropIndex(
                name: "IX_Entradas_BancoId",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "BancoPagadorId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropColumn(
                name: "TipoPagamento",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropColumn(
                name: "BancoId",
                table: "Entradas");
        }
    }
}
