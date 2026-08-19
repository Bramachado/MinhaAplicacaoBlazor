using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFolhaFornecedorItemNotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolhaFornecedorItemNotaId",
                table: "Arquivos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FolhasFornecedoresItensNotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    FolhaFornecedorItemId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolhasFornecedoresItensNotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedoresItensNotas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolhasFornecedoresItensNotas_FolhasFornecedoresItens_FolhaFornecedorItemId",
                        column: x => x.FolhaFornecedorItemId,
                        principalTable: "FolhasFornecedoresItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arquivos_FolhaFornecedorItemNotaId",
                table: "Arquivos",
                column: "FolhaFornecedorItemNotaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItensNotas_EmpresaId",
                table: "FolhasFornecedoresItensNotas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItensNotas_FolhaFornecedorItemId",
                table: "FolhasFornecedoresItensNotas",
                column: "FolhaFornecedorItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arquivos_FolhasFornecedoresItensNotas_FolhaFornecedorItemNotaId",
                table: "Arquivos",
                column: "FolhaFornecedorItemNotaId",
                principalTable: "FolhasFornecedoresItensNotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Backfill: cada item existente vira 1 nota, preservando Documento/Descrição/Valor
            // antes das colunas antigas serem removidas (o pagamento passa a ser a soma das notas).
            migrationBuilder.Sql(@"
                INSERT INTO FolhasFornecedoresItensNotas (EmpresaId, FolhaFornecedorItemId, Descricao, NumeroDocumento, Valor)
                SELECT EmpresaId, Id, Descricao, NumeroDocumento, ValorTotalPagar
                FROM FolhasFornecedoresItens;
            ");

            migrationBuilder.DropColumn(
                name: "NumeroDocumento",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropColumn(
                name: "ValorUnitario",
                table: "FolhasFornecedoresItens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arquivos_FolhasFornecedoresItensNotas_FolhaFornecedorItemNotaId",
                table: "Arquivos");

            migrationBuilder.DropTable(
                name: "FolhasFornecedoresItensNotas");

            migrationBuilder.DropIndex(
                name: "IX_Arquivos_FolhaFornecedorItemNotaId",
                table: "Arquivos");

            migrationBuilder.DropColumn(
                name: "FolhaFornecedorItemNotaId",
                table: "Arquivos");

            migrationBuilder.AddColumn<string>(
                name: "NumeroDocumento",
                table: "FolhasFornecedoresItens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantidade",
                table: "FolhasFornecedoresItens",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorUnitario",
                table: "FolhasFornecedoresItens",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
