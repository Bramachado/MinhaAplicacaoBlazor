using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class FolhaFornecedorItem : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public int FolhaFornecedorId { get; set; }
    public FolhaFornecedor? FolhaFornecedor { get; set; }

    public int FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    /// <summary>Banco da empresa que irá pagar esta despesa.</summary>
    public int? BancoPagadorId { get; set; }
    public Banco? BancoPagador { get; set; }

    /// <summary>Tipo de pagamento desta despesa: "Boleto" ou "Transferencia".</summary>
    [MaxLength(20)]
    public string? TipoPagamento { get; set; }

    /// <summary>Observação geral deste pagamento (não das notas individuais).</summary>
    [MaxLength(200)]
    public string? Descricao { get; set; }

    public DateTime? DataVencimento { get; set; }

    [MaxLength(30)]
    public string? StatusPagamento { get; set; }

    /// <summary>Soma das notas deste pagamento; recalculado a cada nota incluída/removida.</summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotalPagar { get; set; }

    [MaxLength(3)]
    public string? Banco { get; set; }

    [MaxLength(30)]
    public string? Agencia { get; set; }

    [MaxLength(50)]
    public string? Conta { get; set; }

    [MaxLength(150)]
    public string? ChavePix { get; set; }

    [MaxLength(200)]
    public string? NomeTitularConta { get; set; }

    [Column(TypeName = "varchar(max)")]
    public string? Observacao { get; set; }

    /// <summary>Anexo(s) do pagamento consolidado (ex.: comprovante do PIX/boleto único).</summary>
    public List<Arquivo> Arquivos { get; set; } = new();

    /// <summary>Notas fiscais / despesas que compõem este pagamento.</summary>
    public List<FolhaFornecedorItemNota> Notas { get; set; } = new();

    public void Recalcular()
    {
        ValorTotalPagar = Notas.Sum(n => n.Valor);
    }
}
