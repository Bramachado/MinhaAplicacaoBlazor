using Microsoft.AspNetCore.Authorization;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Concede a autorização quando o usuário possui a claim de permissão exigida.
/// As permissões efetivas são carregadas como claims no login
/// (ver <see cref="AppUserClaimsPrincipalFactory"/>).
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var temPermissao = context.User.Claims.Any(c =>
            c.Type == ClaimsPermissao.TipoPermissao &&
            c.Value == requirement.Permissao);

        if (temPermissao)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
