using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Crm;

/// <summary>Situação de uma oportunidade no funil.</summary>
public enum StatusOportunidade
{
    Aberta,
    Ganha,
    Perdida
}

/// <summary>
/// Oportunidade / negócio do CRM. As únicas chaves estrangeiras apontam para
/// outras tabelas do próprio CRM (Contato e Etapa do Funil) — nenhum vínculo
/// com o núcleo do sistema.
/// </summary>
public class CrmOportunidade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public StatusOportunidade Status { get; set; } = StatusOportunidade.Aberta;

    // === Relacionamentos internos do CRM ===
    public int CrmContatoId { get; set; }
    public CrmContato? Contato { get; set; }

    public int CrmEtapaFunilId { get; set; }
    public CrmEtapaFunil? Etapa { get; set; }

    public DateTime? DataPrevisao { get; set; }

    public DateTime? DataFechamento { get; set; }

    [MaxLength(300)]
    public string? MotivoPerda { get; set; }

    [MaxLength(2000)]
    public string? Descricao { get; set; }

    /// <summary>Id do usuário responsável (sem FK — mantém o isolamento).</summary>
    [MaxLength(450)]
    public string? ResponsavelUserId { get; set; }

    /// <summary>Nome do responsável, desnormalizado para exibição sem join.</summary>
    [MaxLength(200)]
    public string? ResponsavelNome { get; set; }

    public DateTime CriadoEm { get; set; }
}
