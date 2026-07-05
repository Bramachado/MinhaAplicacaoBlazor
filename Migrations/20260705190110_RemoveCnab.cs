using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCnab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CnabBatchPayments");

            migrationBuilder.DropTable(
                name: "CnabGeneratedFiles");

            migrationBuilder.DropTable(
                name: "CnabSequences");

            migrationBuilder.DropTable(
                name: "RetornosCnabItens");

            migrationBuilder.DropTable(
                name: "CnabBatches");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CnabBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ambiente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AuditoriaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaminhoZip = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CompetenciaId = table.Column<int>(type: "int", nullable: true),
                    Convenio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmpresaPagadora = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GeradoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeradoPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NomeBaseArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NsaFinal = table.Column<int>(type: "int", nullable: false),
                    NsaInicial = table.Column<int>(type: "int", nullable: false),
                    SeparadoPorForma = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoPagamentoPrincipal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TotalCorrigidos = table.Column<int>(type: "int", nullable: false),
                    TotalInvalidos = table.Column<int>(type: "int", nullable: false),
                    TotalPendentes = table.Column<int>(type: "int", nullable: false),
                    TotalSelecionados = table.Column<int>(type: "int", nullable: false),
                    TotalValidos = table.Column<int>(type: "int", nullable: false),
                    TratamentoInvalidos = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
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
                name: "ConfiguracoesCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Agencia = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    AgenciaDV = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    BancoCodigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    BancoNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Conta = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ContaDV = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Convenio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Layout = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NomeConfiguracao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SequencialArquivo = table.Column<int>(type: "int", nullable: false),
                    VersaoLayoutArquivo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    VersaoLayoutLote = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
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
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Segmentos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasLancamentoCnab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CnabBatchPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CnabBatchId = table.Column<int>(type: "int", nullable: false),
                    ArquivoDestino = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CpfCnpj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataCnab = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormaLancamento = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Origem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrigemId = table.Column<int>(type: "int", nullable: false),
                    SegmentosGerados = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StatusCnab = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ValorCentavos = table.Column<long>(type: "bigint", nullable: false)
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
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Nsa = table.Column<int>(type: "int", nullable: false),
                    QuantidadeOperacoes = table.Column<int>(type: "int", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    TodasLinhas240 = table.Column<bool>(type: "bit", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "RemessasCnab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetenciaId = table.Column<int>(type: "int", nullable: true),
                    ConfiguracaoCnabId = table.Column<int>(type: "int", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConteudoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NumeroSequencial = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuantidadePagamentos = table.Column<int>(type: "int", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
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
                    FolhaColaboradorItemId = table.Column<int>(type: "int", nullable: true),
                    FolhaTutorItemId = table.Column<int>(type: "int", nullable: true),
                    FormaLancamentoCnabId = table.Column<int>(type: "int", nullable: false),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    LancamentoFinanceiroId = table.Column<int>(type: "int", nullable: true),
                    RemessaCnabId = table.Column<int>(type: "int", nullable: false),
                    AgenciaDVFavorecido = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    AgenciaFavorecido = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    BancoFavorecido = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CPF_CNPJ_Favorecido = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChavePix = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CodigoBarras = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: true),
                    CodigoOcorrencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ContaDVFavorecido = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    ContaFavorecido = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MensagemOcorrencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NomeFavorecido = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SeuNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoChavePix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ValorPagamento = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
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
                    ConteudoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataImportacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuantidadeItensProcessados = table.Column<int>(type: "int", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                    RemessaCnabItemId = table.Column<int>(type: "int", nullable: true),
                    RetornoCnabId = table.Column<int>(type: "int", nullable: false),
                    CodigoOcorrencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MensagemOcorrencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NomeFavorecido = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SeuNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StatusProcessamento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorPagamento = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
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
    }
}
