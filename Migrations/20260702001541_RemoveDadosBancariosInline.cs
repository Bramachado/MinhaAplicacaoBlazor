using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDadosBancariosInline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agencia",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "ChavePix",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "CodigoBanco",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "Conta",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "NomeBanco",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "NomeTitular",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "Agencia",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Banco",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "ChavePix",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "CodigoBanco",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Conta",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "NomeTitularConta",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Agencia",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Banco",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "ChavePix",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "CodigoBanco",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Conta",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "NomeTitular",
                table: "Colaboradores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Agencia",
                table: "Tutores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChavePix",
                table: "Tutores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoBanco",
                table: "Tutores",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conta",
                table: "Tutores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeBanco",
                table: "Tutores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeTitular",
                table: "Tutores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agencia",
                table: "Fornecedores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Banco",
                table: "Fornecedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChavePix",
                table: "Fornecedores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoBanco",
                table: "Fornecedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conta",
                table: "Fornecedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeTitularConta",
                table: "Fornecedores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agencia",
                table: "Colaboradores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Banco",
                table: "Colaboradores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChavePix",
                table: "Colaboradores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoBanco",
                table: "Colaboradores",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conta",
                table: "Colaboradores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeTitular",
                table: "Colaboradores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
