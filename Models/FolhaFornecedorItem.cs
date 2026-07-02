using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class FolhaFornecedorItem
{
    public int Id { get; set; }

    public int FolhaFornecedorId { get; set; }
    public FolhaFornecedor? FolhaFornecedor { get; set; }

    public int FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

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

    [MaxLength(500)]
    public string? Observacao { get; set; }

    public List<Arquivo> Arquivos { get; set; } = new();

    public void Recalcular()
    {
        ValorTotalPagar = Quantidade * ValorUnitario;
    }
}
