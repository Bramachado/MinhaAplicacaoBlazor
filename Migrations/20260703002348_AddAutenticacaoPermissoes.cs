using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaAplicacaoBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddAutenticacaoPermissoes : Migration
    {
        // OBSERVAÇÃO: as tabelas do ASP.NET Core Identity (AspNet*) já existiam
        // neste banco (criadas pela migração órfã 20260625145206_IdentityAndDefaults,
        // cujo arquivo não faz mais parte do projeto). Por isso esta migração NÃO
        // recria essas tabelas — apenas ajusta AspNetUsers e cria as tabelas de
        // perfis/permissões. Os IF EXISTS tornam o passo idempotente.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Remove o vínculo antigo AspNetUsers.ColaboradorId (design anterior).
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AspNetUsers_Colaboradores_ColaboradorId')
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Colaboradores_ColaboradorId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUsers_ColaboradorId' AND object_id = OBJECT_ID('AspNetUsers'))
    DROP INDEX [IX_AspNetUsers_ColaboradorId] ON [AspNetUsers];
IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'ColaboradorId' AND object_id = OBJECT_ID('AspNetUsers'))
    ALTER TABLE [AspNetUsers] DROP COLUMN [ColaboradorId];
");

            // 2) Novas colunas do ApplicationUser.
            migrationBuilder.AddColumn<string>(
                name: "NomeCompleto",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "PerfilId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            // 3) Tabelas de perfis e permissões.
            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sistema = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPermissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    Permissao = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPermissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilPermissoes_Perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPermissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Permissao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Concedida = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissoes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 4) Índices e chave estrangeira do perfil.
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PerfilId",
                table: "AspNetUsers",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_Nome",
                table: "Perfis",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermissoes_PerfilId_Permissao",
                table: "PerfilPermissoes",
                columns: new[] { "PerfilId", "Permissao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissoes_UsuarioId_Permissao",
                table: "UsuarioPermissoes",
                columns: new[] { "UsuarioId", "Permissao" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Perfis_PerfilId",
                table: "AspNetUsers",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Perfis_PerfilId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(name: "PerfilPermissoes");
            migrationBuilder.DropTable(name: "UsuarioPermissoes");
            migrationBuilder.DropTable(name: "Perfis");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PerfilId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "NomeCompleto", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Ativo", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "CriadoEm", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "PerfilId", table: "AspNetUsers");

            // Restaura o vínculo antigo com Colaboradores.
            migrationBuilder.AddColumn<int>(
                name: "ColaboradorId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ColaboradorId",
                table: "AspNetUsers",
                column: "ColaboradorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Colaboradores_ColaboradorId",
                table: "AspNetUsers",
                column: "ColaboradorId",
                principalTable: "Colaboradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
