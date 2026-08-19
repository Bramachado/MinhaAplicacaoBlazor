using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Uma nota fiscal / despesa que compõe o pagamento consolidado de um
/// <see cref="FolhaFornecedorItem"/>. O valor total do item é a soma das
/// notas; cada nota pode ter seu próprio anexo (o PDF/imagem da NF).
/// </summary>
public class FolhaFornecedorItemNota : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public int FolhaFornecedorItemId { get; set; }
    public FolhaFornecedorItem? FolhaFornecedorItem { get; set; }

    [MaxLength(200)]
    public string? Descricao { get; set; }

    [MaxLength(50)]
    public string? NumeroDocumento { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Valor { get; set; }

    /// <summary>Número desta parcela (1-based). 1 para nota à vista.</summary>
    public int NumeroParcela { get; set; } = 1;

    /// <summary>Total de parcelas da compra. 1 significa à vista (não parcelada).</summary>
    public int TotalParcelas { get; set; } = 1;

    public List<Arquivo> Arquivos { get; set; } = new();
}
