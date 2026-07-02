using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddEntrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntradaId",
                table: "Arquivos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorBruto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entradas_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arquivos_EntradaId",
                table: "Arquivos",
                column: "EntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_FornecedorId",
                table: "Entradas",
                column: "FornecedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arquivos_Entradas_EntradaId",
                table: "Arquivos",
                column: "EntradaId",
                principalTable: "Entradas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arquivos_Entradas_EntradaId",
                table: "Arquivos");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropIndex(
                name: "IX_Arquivos_EntradaId",
                table: "Arquivos");

            migrationBuilder.DropColumn(
                name: "EntradaId",
                table: "Arquivos");
        }
    }
}
