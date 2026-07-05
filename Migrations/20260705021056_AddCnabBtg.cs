using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddCnabBtg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CnabBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetenciaId = table.Column<int>(type: "int", nullable: true),
                    EmpresaPagadora = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ambiente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Convenio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NsaInicial = table.Column<int>(type: "int", nullable: false),
                    NsaFinal = table.Column<int>(type: "int", nullable: false),
                    NomeBaseArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoPagamentoPrincipal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SeparadoPorForma = table.Column<bool>(type: "bit", nullable: false),
                    TratamentoInvalidos = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalSelecionados = table.Column<int>(type: "int", nullable: false),
                    TotalValidos = table.Column<int>(type: "int", nullable: false),
                    TotalInvalidos = table.Column<int>(type: "int", nullable: false),
                    TotalCorrigidos = table.Column<int>(type: "int", nullable: false),
                    TotalPendentes = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GeradoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeradoPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditoriaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaminhoZip = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnabBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CnabSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaPagadora = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UltimoNsa = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnabSequences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CnabBatchPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CnabBatchId = table.Column<int>(type: "int", nullable: false),
                    Origem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrigemId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CpfCnpj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ValorCentavos = table.Column<long>(type: "bigint", nullable: false),
                    FormaLancamento = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    DataCnab = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SegmentosGerados = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ArquivoDestino = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StatusCnab = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnabBatchPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CnabBatchPayments_CnabBatches_CnabBatchId",
                        column: x => x.CnabBatchId,
                        principalTable: "CnabBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CnabGeneratedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CnabBatchId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nsa = table.Column<int>(type: "int", nullable: false),
                    QuantidadeOperacoes = table.Column<int>(type: "int", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TodasLinhas240 = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnabGeneratedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CnabGeneratedFiles_CnabBatches_CnabBatchId",
                        column: x => x.CnabBatchId,
                        principalTable: "CnabBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CnabBatches_GeradoEm",
                table: "CnabBatches",
                column: "GeradoEm");

            migrationBuilder.CreateIndex(
                name: "IX_CnabBatchPayments_CnabBatchId",
                table: "CnabBatchPayments",
                column: "CnabBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CnabBatchPayments_Origem_OrigemId",
                table: "CnabBatchPayments",
                columns: new[] { "Origem", "OrigemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CnabGeneratedFiles_CnabBatchId",
                table: "CnabGeneratedFiles",
                column: "CnabBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CnabSequences_EmpresaPagadora",
                table: "CnabSequences",
                column: "EmpresaPagadora",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CnabBatchPayments");

            migrationBuilder.DropTable(
                name: "CnabGeneratedFiles");

            migrationBuilder.DropTable(
                name: "CnabSequences");

            migrationBuilder.DropTable(
                name: "CnabBatches");
        }
    }
}
