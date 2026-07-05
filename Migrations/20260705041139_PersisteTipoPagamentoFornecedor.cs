using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class PersisteTipoPagamentoFornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoPagamento",
                table: "Fornecedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Boleto");

            // Backfill dos registros existentes conforme a regra:
            // com conta bancária vinculada → Transferencia; sem conta → Boleto.
            migrationBuilder.Sql(
                "UPDATE [Fornecedores] SET [TipoPagamento] = " +
                "CASE WHEN [ContaBancariaId] IS NOT NULL THEN 'Transferencia' ELSE 'Boleto' END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoPagamento",
                table: "Fornecedores");
        }
    }
}
