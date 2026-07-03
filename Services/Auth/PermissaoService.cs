using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models.Auth;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Calcula o conjunto efetivo de permissões de um usuário:
/// (permissões do perfil) + (concessões individuais) - (revogações individuais).
/// </summary>
public class PermissaoService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PermissaoService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    /// <summary>Retorna as permissões efetivas (chaves do catálogo) do usuário informado.</summary>
    public async Task<HashSet<string>> ObterPermissoesEfetivasAsync(string usuarioId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var usuario = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == usuarioId)
            .Select(u => new { u.PerfilId, u.Ativo })
            .FirstOrDefaultAsync(ct);

        if (usuario is null || !usuario.Ativo)
        {
            return new HashSet<string>();
        }

        var doPerfil = usuario.PerfilId is null
            ? new List<string>()
            : await db.PerfilPermissoes
                .AsNoTracking()
                .Where(p => p.PerfilId == usuario.PerfilId)
                .Select(p => p.Permissao)
                .ToListAsync(ct);

        var ajustes = await db.UsuarioPermissoes
            .AsNoTracking()
            .Where(p => p.UsuarioId == usuarioId)
            .Select(p => new { p.Permissao, p.Concedida })
            .ToListAsync(ct);

        var efetivas = new HashSet<string>(doPerfil);

        foreach (var ajuste in ajustes)
        {
            if (ajuste.Concedida)
                efetivas.Add(ajuste.Permissao);
            else
                efetivas.Remove(ajuste.Permissao);
        }

        // Descarta chaves órfãs (removidas do catálogo) por segurança.
        efetivas.IntersectWith(Permissoes.Todas);
        return efetivas;
    }
}
