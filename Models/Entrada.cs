using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class Entrada
{
    public int Id { get; set; }

    public int FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    public int CompetenciaId { get; set; }
    public Competencia? Competencia { get; set; }

    /// <summary>Banco da empresa associado a esta entrada.</summary>
    public int? BancoId { get; set; }
    public Banco? Banco { get; set; }

    [MaxLength(250)]
    public string? Descricao { get; set; }

    public DateTime DataEmissao { get; set; } = DateTime.Today;

    public DateTime? DataPagamento { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorBruto { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Desconto { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorLiquido { get; set; }

    public List<Arquivo> Arquivos { get; set; } = new();

    public void Recalcular()
    {
        ValorLiquido = ValorBruto - Desconto;
    }
}
