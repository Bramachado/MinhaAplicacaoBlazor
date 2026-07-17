using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Services.Auth;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Fábrica de <see cref="AppDbContext"/> que carimba automaticamente a empresa (tenant)
/// da operação em cada contexto criado. Substitui a fábrica padrão do EF por injeção
/// (registrada por último em Program.cs), de modo que TODO serviço que já injeta
/// <see cref="IDbContextFactory{AppDbContext}"/> passa a receber contextos isolados por
/// empresa — sem qualquer alteração nos serviços/telas existentes.
///
/// A empresa é resolvida uma vez por escopo a partir da claim <c>empresa_id</c> do
/// usuário logado e memorizada em <see cref="TenantContext"/>. Sem usuário autenticado
/// (tela de login, seeding de inicialização) a empresa fica nula e nenhum filtro é aplicado
/// — nesses casos o EmpresaId é definido explicitamente por quem faz a operação.
/// </summary>
public sealed class TenantAwareDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly TenantContext _tenant;
    private readonly AuthenticationStateProvider _authState;

    public TenantAwareDbContextFactory(
        DbContextOptions<AppDbContext> options,
        TenantContext tenant,
        AuthenticationStateProvider authState)
    {
        _options = options;
        _tenant = tenant;
        _authState = authState;
    }

    public AppDbContext CreateDbContext()
    {
        // Caminho síncrono: usado pelo AppDbContext scoped do Identity. As tabelas do
        // Identity não são do tenant, então a empresa (possivelmente nula) é irrelevante aqui.
        var contexto = new AppDbContext(_options);
        contexto.EmpresaIdAtual = _tenant.EmpresaId;
        return contexto;
    }

    public async Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        var contexto = new AppDbContext(_options);
        contexto.EmpresaIdAtual = await ResolverEmpresaAsync();
        return contexto;
    }

    /// <summary>Resolve a empresa do usuário logado (uma vez por escopo) e memoriza.</summary>
    private async Task<int?> ResolverEmpresaAsync()
    {
        if (_tenant.EmpresaId is int cache)
            return cache;

        try
        {
            var estado = await _authState.GetAuthenticationStateAsync();
            var claim = estado.User.FindFirst("empresa_id")?.Value;
            if (int.TryParse(claim, out var id))
                _tenant.EmpresaId = id;
        }
        catch
        {
            // Fora de um circuito/requisição autenticada (ex.: seeding): sem empresa.
        }

        return _tenant.EmpresaId;
    }
}
