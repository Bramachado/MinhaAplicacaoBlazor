using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Crm;

/// <summary>
/// Contato / lead do CRM. Entidade isolada — não possui vínculo (FK) com as
/// tabelas do núcleo do sistema. O "responsável" guarda apenas o id do usuário
/// (sem chave estrangeira) para permitir o filtro "meus contatos".
/// </summary>
public class CrmContato
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(150)]
    public string? EmpresaNome { get; set; }

    [MaxLength(100)]
    public string? Cargo { get; set; }

    /// <summary>Origem do lead (ex.: Indicação, Site, Evento).</summary>
    [MaxLength(60)]
    public string? Origem { get; set; }

    [MaxLength(2000)]
    public string? Observacoes { get; set; }

    /// <summary>Id do usuário responsável (sem FK para o Identity — mantém o isolamento).</summary>
    [MaxLength(450)]
    public string? ResponsavelUserId { get; set; }

    /// <summary>Nome do responsável, desnormalizado para exibição sem join.</summary>
    [MaxLength(200)]
    public string? ResponsavelNome { get; set; }

    public bool Ativo { get; set; } = true;

    /// <summary>Data de criação (preenchida pelo banco com GETDATE()).</summary>
    public DateTime CriadoEm { get; set; }
}
