using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class FolhaFornecedor
{
    public int Id { get; set; }

    public int CompetenciaId { get; set; }
    public Competencia? Competencia { get; set; }

    public DateTime? DataFechamento { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Aberta";

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotal { get; set; }

    public int? LancamentoFinanceiroId { get; set; }
    public LancamentoFinanceiro? LancamentoFinanceiro { get; set; }

    public List<FolhaFornecedorItem> Itens { get; set; } = new();
}
