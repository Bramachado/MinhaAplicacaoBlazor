namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Guarda a empresa (tenant) do escopo atual — um circuito Blazor ou uma requisição.
/// É preenchido a partir da claim <c>empresa_id</c> do usuário logado (ver
/// <see cref="Data.TenantAwareDbContextFactory"/>). No seeding de inicialização é
/// definido manualmente para a empresa padrão.
/// </summary>
public class TenantContext
{
    /// <summary>Empresa do escopo. Null quando não há usuário autenticado (login, seeding inicial).</summary>
    public int? EmpresaId { get; set; }
}
