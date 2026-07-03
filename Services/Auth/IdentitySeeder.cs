using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models.Auth;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Garante que exista o perfil "Administrador" (com todas as permissões) e um
/// usuário administrador inicial para o primeiro acesso.
/// </summary>
public class IdentitySeeder
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public IdentitySeeder(
        IDbContextFactory<AppDbContext> dbFactory,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        _dbFactory = dbFactory;
        _userManager = userManager;
        _config = config;
    }

    public async Task SeedAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        // 1) Perfil Administrador com TODAS as permissões do catálogo.
        var admin = await db.Perfis
            .Include(p => p.Permissoes)
            .FirstOrDefaultAsync(p => p.Nome == "Administrador");

        if (admin is null)
        {
            admin = new Perfil
            {
                Nome = "Administrador",
                Descricao = "Acesso total ao sistema.",
                Sistema = true
            };
            db.Perfis.Add(admin);
            await db.SaveChangesAsync();
        }

        var existentes = admin.Permissoes.Select(p => p.Permissao).ToHashSet();
        var faltantes = Permissoes.Todas.Where(p => !existentes.Contains(p)).ToList();
        if (faltantes.Count > 0)
        {
            foreach (var p in faltantes)
            {
                db.PerfilPermissoes.Add(new PerfilPermissao { PerfilId = admin.Id, Permissao = p });
            }
            await db.SaveChangesAsync();
        }

        // 2) Usuário administrador inicial.
        var adminEmail = _config["AdminInicial:Email"] ?? "admin@edunorte.com.br";
        var adminSenha = _config["AdminInicial:Senha"] ?? "Admin@123456";

        var usuarioAdmin = await _userManager.FindByEmailAsync(adminEmail);
        if (usuarioAdmin is null)
        {
            usuarioAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NomeCompleto = "Administrador",
                Ativo = true,
                PerfilId = admin.Id
            };

            var resultado = await _userManager.CreateAsync(usuarioAdmin, adminSenha);
            if (!resultado.Succeeded)
            {
                var erros = string.Join("; ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Falha ao criar admin inicial: {erros}");
            }
        }
        else if (usuarioAdmin.PerfilId is null)
        {
            usuarioAdmin.PerfilId = admin.Id;
            await _userManager.UpdateAsync(usuarioAdmin);
        }
    }
}
