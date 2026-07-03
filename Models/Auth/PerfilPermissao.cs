namespace MinhaAplicacaoBlazor.Models.Auth;

/// <summary>
/// Uma permissão concedida por um perfil. O valor é uma chave do catálogo
/// <see cref="Permissoes"/>, ex.: "Fornecedores.Editar".
/// </summary>
public class PerfilPermissao
{
    public int Id { get; set; }

    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;

    public string Permissao { get; set; } = string.Empty;
}
