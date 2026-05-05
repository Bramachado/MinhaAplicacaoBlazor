using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomeCurso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CargaHorariaTotal = table.Column<int>(type: "int", nullable: false),
                    TotalAnos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO Cursos (Codigo, NomeCurso, CargaHorariaTotal, TotalAnos)
                VALUES ('PADRAO', 'Curso Padrão', 0, 0);
            ");

            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "Tutores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE Tutores
                SET CursoId = (SELECT TOP 1 Id FROM Cursos WHERE Codigo = 'PADRAO');
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_CursoId",
                table: "Tutores",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_Cursos_CursoId",
                table: "Tutores",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_Cursos_CursoId",
                table: "Tutores");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_Tutores_CursoId",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "Tutores");
        }
    }
}
