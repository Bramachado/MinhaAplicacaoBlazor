namespace MinhaAplicacaoBlazor.Models.Auth;

/// <summary>
/// Perfil (papel) reutilizável — agrupa um conjunto de permissões.
/// Ex.: "Administrador", "Financeiro", "RH".
/// </summary>
public class Perfil
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    /// <summary>
    /// Perfil de sistema (ex.: Administrador) não pode ser excluído.
    /// </summary>
    public bool Sistema { get; set; }

    public ICollection<PerfilPermissao> Permissoes { get; set; } = new List<PerfilPermissao>();

    public ICollection<ApplicationUser> Usuarios { get; set; } = new List<ApplicationUser>();
}
