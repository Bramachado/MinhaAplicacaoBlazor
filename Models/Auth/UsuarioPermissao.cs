namespace MinhaAplicacaoBlazor.Models.Auth;

/// <summary>
/// Ajuste individual de permissão, sobrepondo o que vem do perfil.
/// Permite conceder algo a mais (<see cref="Concedida"/> = true) ou
/// revogar algo que o perfil concede (<see cref="Concedida"/> = false).
/// </summary>
public class UsuarioPermissao
{
    public int Id { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser Usuario { get; set; } = null!;

    public string Permissao { get; set; } = string.Empty;

    /// <summary>true = concede (mesmo sem o perfil); false = revoga (mesmo com o perfil).</summary>
    public bool Concedida { get; set; }
}
