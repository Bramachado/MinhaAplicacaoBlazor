using Microsoft.AspNetCore.Authorization;

namespace MinhaAplicacaoBlazor.Services.Auth;

/// <summary>
/// Requisito de autorização que exige uma permissão específica do catálogo,
/// ex.: "Fornecedores.Editar".
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permissao) => Permissao = permissao;

    public string Permissao { get; }
}
