using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Crm;

/// <summary>
/// Estágio do funil de vendas do CRM (ex.: Novo, Proposta, Negociação).
/// Entidade isolada — não possui vínculo com as tabelas do núcleo do sistema.
/// </summary>
public class CrmEtapaFunil
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da etapa é obrigatório.")]
    [MaxLength(80)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Ordem de exibição no funil (menor primeiro).</summary>
    public int Ordem { get; set; }

    /// <summary>Cor de destaque no Kanban (hex, ex.: "#0d6efd").</summary>
    [MaxLength(20)]
    public string? Cor { get; set; }

    public bool Ativa { get; set; } = true;
}
