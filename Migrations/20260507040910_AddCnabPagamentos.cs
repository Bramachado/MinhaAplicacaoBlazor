using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddCnabPagamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeConfiguracao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BancoCodigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    BancoNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Agencia = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    AgenciaDV = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Conta = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ContaDV = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Convenio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Layout = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VersaoLayoutArquivo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    VersaoLayoutLote = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SequencialArquivo = table.Column<int>(type: "int", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesCnab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormasLancamentoCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Segmentos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasLancamentoCnab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RemessasCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfiguracaoCnabId = table.Column<int>(type: "int", nullable: false),
                    CompetenciaId = table.Column<int>(type: "int", nullable: true),
                    NumeroSequencial = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    QuantidadePagamentos = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConteudoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemessasCnab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemessasCnab_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RemessasCnab_ConfiguracoesCnab_ConfiguracaoCnabId",
                        column: x => x.ConfiguracaoCnabId,
                        principalTable: "ConfiguracoesCnab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RemessasCnabItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemessaCnabId = table.Column<int>(type: "int", nullable: false),
                    LancamentoFinanceiroId = table.Column<int>(type: "int", nullable: true),
                    FolhaColaboradorItemId = table.Column<int>(type: "int", nullable: true),
                    FolhaTutorItemId = table.Column<int>(type: "int", nullable: true),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    FormaLancamentoCnabId = table.Column<int>(type: "int", nullable: false),
                    NomeFavorecido = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CPF_CNPJ_Favorecido = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BancoFavorecido = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AgenciaFavorecido = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    AgenciaDVFavorecido = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    ContaFavorecido = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    ContaDVFavorecido = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    ChavePix = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TipoChavePix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CodigoBarras = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorPagamento = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    SeuNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodigoOcorrencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MensagemOcorrencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemessasCnabItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_FolhasColaboradoresItens_FolhaColaboradorItemId",
                        column: x => x.FolhaColaboradorItemId,
                        principalTable: "FolhasColaboradoresItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_FolhasTutoresItens_FolhaTutorItemId",
                        column: x => x.FolhaTutorItemId,
                        principalTable: "FolhasTutoresItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_FormasLancamentoCnab_FormaLancamentoCnabId",
                        column: x => x.FormaLancamentoCnabId,
                        principalTable: "FormasLancamentoCnab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_LancamentosFinanceiros_LancamentoFinanceiroId",
                        column: x => x.LancamentoFinanceiroId,
                        principalTable: "LancamentosFinanceiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RemessasCnabItens_RemessasCnab_RemessaCnabId",
                        column: x => x.RemessaCnabId,
                        principalTable: "RemessasCnab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetornosCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemessaCnabId = table.Column<int>(type: "int", nullable: true),
                    NomeArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataImportacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConteudoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    QuantidadeItensProcessados = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetornosCnab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetornosCnab_RemessasCnab_RemessaCnabId",
                        column: x => x.RemessaCnabId,
                        principalTable: "RemessasCnab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RetornosCnabItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetornoCnabId = table.Column<int>(type: "int", nullable: false),
                    RemessaCnabItemId = table.Column<int>(type: "int", nullable: true),
                    SeuNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeFavorecido = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValorPagamento = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CodigoOcorrencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MensagemOcorrencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StatusProcessamento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetornosCnabItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetornosCnabItens_RemessasCnabItens_RemessaCnabItemId",
                        column: x => x.RemessaCnabItemId,
                        principalTable: "RemessasCnabItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RetornosCnabItens_RetornosCnab_RetornoCnabId",
                        column: x => x.RetornoCnabId,
                        principalTable: "RetornosCnab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesCnab_NomeConfiguracao",
                table: "ConfiguracoesCnab",
                column: "NomeConfiguracao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormasLancamentoCnab_Codigo",
                table: "FormasLancamentoCnab",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnab_CompetenciaId",
                table: "RemessasCnab",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnab_ConfiguracaoCnabId",
                table: "RemessasCnab",
                column: "ConfiguracaoCnabId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnab_NomeArquivo",
                table: "RemessasCnab",
                column: "NomeArquivo");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnab_NumeroSequencial",
                table: "RemessasCnab",
                column: "NumeroSequencial");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_FolhaColaboradorItemId",
                table: "RemessasCnabItens",
                column: "FolhaColaboradorItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_FolhaTutorItemId",
                table: "RemessasCnabItens",
                column: "FolhaTutorItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_FormaLancamentoCnabId",
                table: "RemessasCnabItens",
                column: "FormaLancamentoCnabId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_FornecedorId",
                table: "RemessasCnabItens",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_LancamentoFinanceiroId",
                table: "RemessasCnabItens",
                column: "LancamentoFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_RemessaCnabId",
                table: "RemessasCnabItens",
                column: "RemessaCnabId");

            migrationBuilder.CreateIndex(
                name: "IX_RemessasCnabItens_SeuNumero",
                table: "RemessasCnabItens",
                column: "SeuNumero");

            migrationBuilder.CreateIndex(
                name: "IX_RetornosCnab_RemessaCnabId",
                table: "RetornosCnab",
                column: "RemessaCnabId");

            migrationBuilder.CreateIndex(
                name: "IX_RetornosCnabItens_RemessaCnabItemId",
                table: "RetornosCnabItens",
                column: "RemessaCnabItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RetornosCnabItens_RetornoCnabId",
                table: "RetornosCnabItens",
                column: "RetornoCnabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetornosCnabItens");

            migrationBuilder.DropTable(
                name: "RemessasCnabItens");

            migrationBuilder.DropTable(
                name: "RetornosCnab");

            migrationBuilder.DropTable(
                name: "FormasLancamentoCnab");

            migrationBuilder.DropTable(
                name: "RemessasCnab");

            migrationBuilder.DropTable(
                name: "ConfiguracoesCnab");
        }
    }
}
