using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MinhaAplicacaoBlazor.Models.Auth;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Cria policies dinamicamente para cada permissão do catálogo, sem precisar
/// registrar uma a uma. Assim, [Authorize(Policy = "Fornecedores.Editar")] e
/// &lt;AuthorizeView Policy="Fornecedores.Editar"&gt; funcionam para qualquer chave válida.
/// </summary>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        => _fallback = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (Permissoes.Existe(policyName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}
