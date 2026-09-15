using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddSituacaoCpfTutorColaborador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SituacaoCpf",
                table: "Tutores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SituacaoCpf",
                table: "Colaboradores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SituacaoCpf",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "SituacaoCpf",
                table: "Colaboradores");
        }
    }
}
