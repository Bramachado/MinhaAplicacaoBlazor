using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Helpers para criar um <see cref="AppDbContext"/> já carimbado com o usuário
/// logado, de modo que a auditoria automática saiba "quem" fez a operação.
/// Em Blazor Server interativo o IHttpContextAccessor é nulo durante o circuito,
/// por isso o usuário é passado explicitamente a partir do AuthenticationState.
/// </summary>
public static class AuditoriaContextoExtensions
{
    /// <summary>
    /// Cria um contexto e define o usuário de auditoria a partir do ClaimsPrincipal.
    /// </summary>
    public static async Task<AppDbContext> CreateDbContextAsync(
        this IDbContextFactory<AppDbContext> factory,
        ClaimsPrincipal? usuario)
    {
        var contexto = await factory.CreateDbContextAsync();
        contexto.DefinirUsuarioAuditoria(usuario);
        return contexto;
    }

    /// <summary>Carimba o contexto com o usuário logado (id + nome de exibição).</summary>
    public static void DefinirUsuarioAuditoria(this AppDbContext contexto, ClaimsPrincipal? usuario)
    {
        if (usuario?.Identity?.IsAuthenticated == true)
        {
            contexto.UsuarioAuditoriaId = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            contexto.UsuarioAuditoriaNome =
                usuario.FindFirst("nome_completo")?.Value
                ?? usuario.Identity.Name
                ?? "Desconhecido";
        }
    }
}
