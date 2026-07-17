using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unidades_Codigo",
                table: "Unidades");

            migrationBuilder.DropIndex(
                name: "IX_Tutores_Cpf",
                table: "Tutores");

            migrationBuilder.DropIndex(
                name: "IX_PlanosContas_Nome",
                table: "PlanosContas");

            migrationBuilder.DropIndex(
                name: "IX_FormasPagamento_Nome",
                table: "FormasPagamento");

            migrationBuilder.DropIndex(
                name: "IX_Competencias_Mes_Ano",
                table: "Competencias");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_Cpf",
                table: "Colaboradores");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasFornecedores_Nome",
                table: "CategoriasFornecedores");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Unidades",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Tutores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Titulacoes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "PlanosContas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "LancamentosFinanceiros",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Fornecedores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FormasPagamento",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasTutoresItens",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasTutores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasFornecedoresItens",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasFornecedores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasColaboradoresItens",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "FolhasColaboradores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "EscalasAulasPraticas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Entradas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Cursos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "ContasBancarias",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Competencias",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Colaboradores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "CategoriasFornecedores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Arquivos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            // Empresa (tenant) padrão. Todas as linhas pré-existentes passam a pertencer
            // a ela (as colunas EmpresaId voltaram com defaultValue 1). Precisa existir
            // ANTES das FKs abaixo, senão elas violam a integridade referencial.
            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Nome", "Cnpj", "Ativa", "CriadoEm" },
                values: new object[] { 1, "Empresa Padrão", null, true, new DateTime(2026, 1, 1) });

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_EmpresaId_Codigo",
                table: "Unidades",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_EmpresaId_Cpf",
                table: "Tutores",
                columns: new[] { "EmpresaId", "Cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Titulacoes_EmpresaId",
                table: "Titulacoes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosContas_EmpresaId_Nome",
                table: "PlanosContas",
                columns: new[] { "EmpresaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosFinanceiros_EmpresaId",
                table: "LancamentosFinanceiros",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_EmpresaId",
                table: "Fornecedores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FormasPagamento_EmpresaId_Nome",
                table: "FormasPagamento",
                columns: new[] { "EmpresaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutoresItens_EmpresaId",
                table: "FolhasTutoresItens",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasTutores_EmpresaId",
                table: "FolhasTutores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedoresItens_EmpresaId",
                table: "FolhasFornecedoresItens",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasFornecedores_EmpresaId",
                table: "FolhasFornecedores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradoresItens_EmpresaId",
                table: "FolhasColaboradoresItens",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FolhasColaboradores_EmpresaId",
                table: "FolhasColaboradores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalasAulasPraticas_EmpresaId",
                table: "EscalasAulasPraticas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_EmpresaId",
                table: "Entradas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_EmpresaId",
                table: "Cursos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContasBancarias_EmpresaId",
                table: "ContasBancarias",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Competencias_EmpresaId_Mes_Ano",
                table: "Competencias",
                columns: new[] { "EmpresaId", "Mes", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_EmpresaId_Cpf",
                table: "Colaboradores",
                columns: new[] { "EmpresaId", "Cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasFornecedores_EmpresaId_Nome",
                table: "CategoriasFornecedores",
                columns: new[] { "EmpresaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Arquivos_EmpresaId",
                table: "Arquivos",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arquivos_Empresas_EmpresaId",
                table: "Arquivos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriasFornecedores_Empresas_EmpresaId",
                table: "CategoriasFornecedores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Colaboradores_Empresas_EmpresaId",
                table: "Colaboradores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Competencias_Empresas_EmpresaId",
                table: "Competencias",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContasBancarias_Empresas_EmpresaId",
                table: "ContasBancarias",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_Empresas_EmpresaId",
                table: "Cursos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Empresas_EmpresaId",
                table: "Entradas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EscalasAulasPraticas_Empresas_EmpresaId",
                table: "EscalasAulasPraticas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasColaboradores_Empresas_EmpresaId",
                table: "FolhasColaboradores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasColaboradoresItens_Empresas_EmpresaId",
                table: "FolhasColaboradoresItens",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasFornecedores_Empresas_EmpresaId",
                table: "FolhasFornecedores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasFornecedoresItens_Empresas_EmpresaId",
                table: "FolhasFornecedoresItens",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasTutores_Empresas_EmpresaId",
                table: "FolhasTutores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FolhasTutoresItens_Empresas_EmpresaId",
                table: "FolhasTutoresItens",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormasPagamento_Empresas_EmpresaId",
                table: "FormasPagamento",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedores_Empresas_EmpresaId",
                table: "Fornecedores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentosFinanceiros_Empresas_EmpresaId",
                table: "LancamentosFinanceiros",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanosContas_Empresas_EmpresaId",
                table: "PlanosContas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Titulacoes_Empresas_EmpresaId",
                table: "Titulacoes",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_Empresas_EmpresaId",
                table: "Tutores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Unidades_Empresas_EmpresaId",
                table: "Unidades",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arquivos_Empresas_EmpresaId",
                table: "Arquivos");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriasFornecedores_Empresas_EmpresaId",
                table: "CategoriasFornecedores");

            migrationBuilder.DropForeignKey(
                name: "FK_Colaboradores_Empresas_EmpresaId",
                table: "Colaboradores");

            migrationBuilder.DropForeignKey(
                name: "FK_Competencias_Empresas_EmpresaId",
                table: "Competencias");

            migrationBuilder.DropForeignKey(
                name: "FK_ContasBancarias_Empresas_EmpresaId",
                table: "ContasBancarias");

            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_Empresas_EmpresaId",
                table: "Cursos");

            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Empresas_EmpresaId",
                table: "Entradas");

            migrationBuilder.DropForeignKey(
                name: "FK_EscalasAulasPraticas_Empresas_EmpresaId",
                table: "EscalasAulasPraticas");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasColaboradores_Empresas_EmpresaId",
                table: "FolhasColaboradores");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasColaboradoresItens_Empresas_EmpresaId",
                table: "FolhasColaboradoresItens");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasFornecedores_Empresas_EmpresaId",
                table: "FolhasFornecedores");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasFornecedoresItens_Empresas_EmpresaId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasTutores_Empresas_EmpresaId",
                table: "FolhasTutores");

            migrationBuilder.DropForeignKey(
                name: "FK_FolhasTutoresItens_Empresas_EmpresaId",
                table: "FolhasTutoresItens");

            migrationBuilder.DropForeignKey(
                name: "FK_FormasPagamento_Empresas_EmpresaId",
                table: "FormasPagamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedores_Empresas_EmpresaId",
                table: "Fornecedores");

            migrationBuilder.DropForeignKey(
                name: "FK_LancamentosFinanceiros_Empresas_EmpresaId",
                table: "LancamentosFinanceiros");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanosContas_Empresas_EmpresaId",
                table: "PlanosContas");

            migrationBuilder.DropForeignKey(
                name: "FK_Titulacoes_Empresas_EmpresaId",
                table: "Titulacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_Empresas_EmpresaId",
                table: "Tutores");

            migrationBuilder.DropForeignKey(
                name: "FK_Unidades_Empresas_EmpresaId",
                table: "Unidades");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropIndex(
                name: "IX_Unidades_EmpresaId_Codigo",
                table: "Unidades");

            migrationBuilder.DropIndex(
                name: "IX_Tutores_EmpresaId_Cpf",
                table: "Tutores");

            migrationBuilder.DropIndex(
                name: "IX_Titulacoes_EmpresaId",
                table: "Titulacoes");

            migrationBuilder.DropIndex(
                name: "IX_PlanosContas_EmpresaId_Nome",
                table: "PlanosContas");

            migrationBuilder.DropIndex(
                name: "IX_LancamentosFinanceiros_EmpresaId",
                table: "LancamentosFinanceiros");

            migrationBuilder.DropIndex(
                name: "IX_Fornecedores_EmpresaId",
                table: "Fornecedores");

            migrationBuilder.DropIndex(
                name: "IX_FormasPagamento_EmpresaId_Nome",
                table: "FormasPagamento");

            migrationBuilder.DropIndex(
                name: "IX_FolhasTutoresItens_EmpresaId",
                table: "FolhasTutoresItens");

            migrationBuilder.DropIndex(
                name: "IX_FolhasTutores_EmpresaId",
                table: "FolhasTutores");

            migrationBuilder.DropIndex(
                name: "IX_FolhasFornecedoresItens_EmpresaId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropIndex(
                name: "IX_FolhasFornecedores_EmpresaId",
                table: "FolhasFornecedores");

            migrationBuilder.DropIndex(
                name: "IX_FolhasColaboradoresItens_EmpresaId",
                table: "FolhasColaboradoresItens");

            migrationBuilder.DropIndex(
                name: "IX_FolhasColaboradores_EmpresaId",
                table: "FolhasColaboradores");

            migrationBuilder.DropIndex(
                name: "IX_EscalasAulasPraticas_EmpresaId",
                table: "EscalasAulasPraticas");

            migrationBuilder.DropIndex(
                name: "IX_Entradas_EmpresaId",
                table: "Entradas");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_EmpresaId",
                table: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_ContasBancarias_EmpresaId",
                table: "ContasBancarias");

            migrationBuilder.DropIndex(
                name: "IX_Competencias_EmpresaId_Mes_Ano",
                table: "Competencias");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_EmpresaId_Cpf",
                table: "Colaboradores");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasFornecedores_EmpresaId_Nome",
                table: "CategoriasFornecedores");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Arquivos_EmpresaId",
                table: "Arquivos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Unidades");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Titulacoes");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "PlanosContas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "LancamentosFinanceiros");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FormasPagamento");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasTutoresItens");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasTutores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasFornecedoresItens");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasFornecedores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasColaboradoresItens");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "FolhasColaboradores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "EscalasAulasPraticas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "ContasBancarias");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Competencias");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "CategoriasFornecedores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Arquivos");

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_Codigo",
                table: "Unidades",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_Cpf",
                table: "Tutores",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanosContas_Nome",
                table: "PlanosContas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormasPagamento_Nome",
                table: "FormasPagamento",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Competencias_Mes_Ano",
                table: "Competencias",
                columns: new[] { "Mes", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_Cpf",
                table: "Colaboradores",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasFornecedores_Nome",
                table: "CategoriasFornecedores",
                column: "Nome",
                unique: true);
        }
    }
}
