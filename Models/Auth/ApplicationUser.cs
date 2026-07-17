using Microsoft.AspNetCore.Identity;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Models.Auth;

/// <summary>
/// Usuário da aplicação. Estende o IdentityUser padrão com dados do ERP.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>Empresa (tenant) à qual o usuário pertence. Define quais registros ele enxerga.</summary>
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    /// <summary>Quando falso, o usuário existe mas não consegue entrar.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Perfil (papel) principal do usuário. Define o conjunto base de permissões.</summary>
    public int? PerfilId { get; set; }
    public Perfil? Perfil { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    /// <summary>Permissões concedidas/revogadas individualmente, sobrepondo o perfil.</summary>
    public ICollection<UsuarioPermissao> Permissoes { get; set; } = new List<UsuarioPermissao>();
}
