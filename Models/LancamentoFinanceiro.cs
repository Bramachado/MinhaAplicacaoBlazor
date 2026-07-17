using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class LancamentoFinanceiro : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public int CompetenciaId { get; set; }
    public Competencia? Competencia { get; set; }

    public int? UnidadeId { get; set; }
    public Unidade? Unidade { get; set; }

    public int PlanoContaId { get; set; }
    public PlanoConta? PlanoConta { get; set; }

    public int? FormaPagamentoId { get; set; }
    public FormaPagamento? FormaPagamento { get; set; }

    [Required(ErrorMessage = "Fornecero é obrigatório.")]
    public int? FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    [Required(ErrorMessage = "O tipo de lançamento é obrigatório.")]
    [MaxLength(20)]
    public string TipoLancamento { get; set; } = "Saida";

    [MaxLength(20)]
    public string? TipoDespesa { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Column(TypeName = "decimal(12,2)")]
    [Range(0.01, 999999999999.99, ErrorMessage = "Informe um valor maior que zero.")]
    public decimal Valor { get; set; }

    [MaxLength(500)]
    public string? Observacao { get; set; }

    [Required]
    [MaxLength(50)]
    public string Origem { get; set; } = "Manual";

    public int? OrigemId { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataVencimento { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataPagamento { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Aberto";

    public DateTime CriadoEm { get; set; } = DateTime.Now;
}
