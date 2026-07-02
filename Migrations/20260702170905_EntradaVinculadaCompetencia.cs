using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class EntradaVinculadaCompetencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompetenciaId",
                table: "Entradas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Backfill: vincula entradas já existentes à competência do mês/ano da emissão
            // (ou à competência mais recente, caso não haja correspondência exata).
            migrationBuilder.Sql(@"
                UPDATE e
                SET e.CompetenciaId = COALESCE(
                    (SELECT TOP 1 c.Id FROM Competencias c
                     WHERE c.Mes = MONTH(e.DataEmissao) AND c.Ano = YEAR(e.DataEmissao)),
                    (SELECT TOP 1 c2.Id FROM Competencias c2 ORDER BY c2.Ano DESC, c2.Mes DESC))
                FROM Entradas e
                WHERE e.CompetenciaId = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_CompetenciaId",
                table: "Entradas",
                column: "CompetenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Competencias_CompetenciaId",
                table: "Entradas",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Competencias_CompetenciaId",
                table: "Entradas");

            migrationBuilder.DropIndex(
                name: "IX_Entradas_CompetenciaId",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "Entradas");
        }
    }
}
