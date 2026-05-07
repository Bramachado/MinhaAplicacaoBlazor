using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFolhaTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolhasTutores",
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
                    table.PrimaryKey("PK_FolhasTutores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasTutores_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolhasTutores_LancamentosFinanceiros_LancamentoFinanceiroId",
                        column: x => x.LancamentoFinanceiroId,
                        principalTable: "LancamentosFinanceiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FolhasTutoresItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolhaTutorId = table.Column<int>(type: "int", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatusPagamento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TotalHorasNormais = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalHorasPraticas = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorHoraNormal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorHoraPratica = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorAulaNormal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorAulaPratica = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorTotalReceber = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Agencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Conta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChavePix = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NomeTitularConta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolhasTutoresItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasTutoresItens_FolhasTutores_FolhaTutorId",
                        column: x => x.FolhaTutorId,
                        principalTable: "FolhasTutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolhasTutoresItens_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutores_CompetenciaId",
                table: "FolhasTutores",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutores_LancamentoFinanceiroId",
                table: "FolhasTutores",
                column: "LancamentoFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutoresItens_FolhaTutorId",
                table: "FolhasTutoresItens",
                column: "FolhaTutorId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutoresItens_TutorId",
                table: "FolhasTutoresItens",
                column: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolhasTutoresItens");

            migrationBuilder.DropTable(
                name: "FolhasTutores");
        }
    }
}
