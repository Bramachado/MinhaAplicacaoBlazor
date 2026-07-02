using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddContaBancaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaBancariaId",
                table: "Tutores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContaBancariaId",
                table: "Fornecedores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContaBancariaId",
                table: "Colaboradores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContasBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeTitular = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CpfCnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    ChavePix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoBanco = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    NomeBanco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Agencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Conta = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasBancarias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_ContaBancariaId",
                table: "Tutores",
                column: "ContaBancariaId",
                unique: true,
                filter: "[ContaBancariaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_ContaBancariaId",
                table: "Fornecedores",
                column: "ContaBancariaId",
                unique: true,
                filter: "[ContaBancariaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_ContaBancariaId",
                table: "Colaboradores",
                column: "ContaBancariaId",
                unique: true,
                filter: "[ContaBancariaId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Colaboradores_ContasBancarias_ContaBancariaId",
                table: "Colaboradores",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedores_ContasBancarias_ContaBancariaId",
                table: "Fornecedores",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_ContasBancarias_ContaBancariaId",
                table: "Tutores",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colaboradores_ContasBancarias_ContaBancariaId",
                table: "Colaboradores");

            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedores_ContasBancarias_ContaBancariaId",
                table: "Fornecedores");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_ContasBancarias_ContaBancariaId",
                table: "Tutores");

            migrationBuilder.DropTable(
                name: "ContasBancarias");

            migrationBuilder.DropIndex(
                name: "IX_Tutores_ContaBancariaId",
                table: "Tutores");

            migrationBuilder.DropIndex(
                name: "IX_Fornecedores_ContaBancariaId",
                table: "Fornecedores");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_ContaBancariaId",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "ContaBancariaId",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "ContaBancariaId",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "ContaBancariaId",
                table: "Colaboradores");
        }
    }
}
