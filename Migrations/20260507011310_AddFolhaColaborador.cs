using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFolhaColaborador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolhasColaboradores",
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
                    table.PrimaryKey("PK_FolhasColaboradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasColaboradores_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolhasColaboradores_LancamentosFinanceiros_LancamentoFinanceiroId",
                        column: x => x.LancamentoFinanceiroId,
                        principalTable: "LancamentosFinanceiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FolhasColaboradoresItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolhaColaboradorId = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CargaHorariaSemanal = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SalarioBruto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TicketAlimentacao = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SalarioLiquidoBase = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SalarioFamilia = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PremiacaoFixa = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PremiacaoVariavel = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DecimoTerceiro = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FeriasUmTerco = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalProventos = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorHora = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    HorasFalta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorFaltas = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DescontoINSS = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DescontoIR = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PlanoSaude = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValeConsignadoFGTS = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalDescontos = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ValorReceberPix = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolhasColaboradoresItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasColaboradoresItens_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolhasColaboradoresItens_FolhasColaboradores_FolhaColaboradorId",
                        column: x => x.FolhaColaboradorId,
                        principalTable: "FolhasColaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradores_CompetenciaId",
                table: "FolhasColaboradores",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradores_LancamentoFinanceiroId",
                table: "FolhasColaboradores",
                column: "LancamentoFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradoresItens_ColaboradorId",
                table: "FolhasColaboradoresItens",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradoresItens_FolhaColaboradorId",
                table: "FolhasColaboradoresItens",
                column: "FolhaColaboradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolhasColaboradoresItens");

            migrationBuilder.DropTable(
                name: "FolhasColaboradores");
        }
    }
}
