using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFolhaFornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolhasFornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetenciaId = table.Column<int>(type: "int", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Aberta"),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    LancamentoFinanceiroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolhasFornecedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedores_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedores_LancamentosFinanceiros_LancamentoFinanceiroId",
                        column: x => x.LancamentoFinanceiroId,
                        principalTable: "LancamentosFinanceiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FolhasFornecedoresItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolhaFornecedorId = table.Column<int>(type: "int", nullable: false),
                    FornecedorId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusPagamento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Quantidade = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorTotalPagar = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Agencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Conta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChavePix = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NomeTitularConta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolhasFornecedoresItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedoresItens_FolhasFornecedores_FolhaFornecedorId",
                        column: x => x.FolhaFornecedorId,
                        principalTable: "FolhasFornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedoresItens_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedores_CompetenciaId",
                table: "FolhasFornecedores",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedores_LancamentoFinanceiroId",
                table: "FolhasFornecedores",
                column: "LancamentoFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItens_FolhaFornecedorId",
                table: "FolhasFornecedoresItens",
                column: "FolhaFornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItens_FornecedorId",
                table: "FolhasFornecedoresItens",
                column: "FornecedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolhasFornecedoresItens");

            migrationBuilder.DropTable(
                name: "FolhasFornecedores");
        }
    }
}
