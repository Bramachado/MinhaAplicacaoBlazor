using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MinhaAplicacaoBlazor.Models.Auth;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Ao construir a identidade do usuário (no login), injeta uma claim por permissão
/// efetiva, além do nome de exibição. As permissões ficam gravadas no cookie de
/// autenticação — mudanças de perfil/permissão passam a valer no próximo login.
/// </summary>
public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    private readonly PermissaoService _permissoes;

    public AppUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        PermissaoService permissoes)
        : base(userManager, optionsAccessor)
    {
        _permissoes = permissoes;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (!string.IsNullOrEmpty(user.NomeCompleto))
        {
            identity.AddClaim(new Claim("nome_completo", user.NomeCompleto));
        }

        var efetivas = await _permissoes.ObterPermissoesEfetivasAsync(user.Id);
        foreach (var permissao in efetivas)
        {
            identity.AddClaim(new Claim(ClaimsPermissao.TipoPermissao, permissao));
        }

        return identity;
    }
}
