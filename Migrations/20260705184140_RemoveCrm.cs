using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCrm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oportunidades",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Contatos",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "EtapasFunil",
                schema: "crm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "crm");

            migrationBuilder.CreateTable(
                name: "Contatos",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EmpresaNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Origem = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ResponsavelNome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsavelUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtapasFunil",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oportunidades",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrmContatoId = table.Column<int>(type: "int", nullable: false),
                    CrmEtapaFunilId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPrevisao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MotivoPerda = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ResponsavelNome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsavelUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oportunidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oportunidades_Contatos_CrmContatoId",
                        column: x => x.CrmContatoId,
                        principalSchema: "crm",
                        principalTable: "Contatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oportunidades_EtapasFunil_CrmEtapaFunilId",
                        column: x => x.CrmEtapaFunilId,
                        principalSchema: "crm",
                        principalTable: "EtapasFunil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contatos_Nome",
                schema: "crm",
                table: "Contatos",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Contatos_ResponsavelUserId",
                schema: "crm",
                table: "Contatos",
                column: "ResponsavelUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapasFunil_Ordem",
                schema: "crm",
                table: "EtapasFunil",
                column: "Ordem");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_CrmContatoId",
                schema: "crm",
                table: "Oportunidades",
                column: "CrmContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_CrmEtapaFunilId",
                schema: "crm",
                table: "Oportunidades",
                column: "CrmEtapaFunilId");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_ResponsavelUserId",
                schema: "crm",
                table: "Oportunidades",
                column: "ResponsavelUserId");
        }
    }
}
