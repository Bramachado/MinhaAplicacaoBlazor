using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class TornaUnidadeIdLancamentoNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O lançamento gerado por uma folha agrega vários colaboradores/tutores,
            // portanto não possui uma única unidade (UnidadeId deve permitir NULL).
            migrationBuilder.Sql(
                "ALTER TABLE [LancamentosFinanceiros] ALTER COLUMN [UnidadeId] int NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE [LancamentosFinanceiros] ALTER COLUMN [UnidadeId] int NOT NULL;");
        }
    }
}
