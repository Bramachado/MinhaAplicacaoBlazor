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

    [MaxLength(200)]
    public string? Descricao { get; set; }

    [MaxLength(50)]
    public string? NumeroDocumento { get; set; }

    public DateTime? DataVencimento { get; set; }

    [MaxLength(30)]
    public string? StatusPagamento { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Quantidade { get; set; } = 1m;

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorUnitario { get; set; }

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

    public List<Arquivo> Arquivos { get; set; } = new();

    public void Recalcular()
    {
        ValorTotalPagar = Quantidade * ValorUnitario;
    }
}
