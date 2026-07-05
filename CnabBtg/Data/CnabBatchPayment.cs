using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.CnabBtg.Data;

/// <summary>
/// Vínculo de um pagamento (origem + id de origem) a um <see cref="CnabBatch"/>,
/// com sua classificação. Usado para impedir reinclusão de um pagamento já
/// presente em um CNAB ativo e para o histórico.
/// </summary>
public class CnabBatchPayment
{
    public int Id { get; set; }

    public int CnabBatchId { get; set; }
    public CnabBatch? CnabBatch { get; set; }

    /// <summary>Origem: Colaborador / Tutor / Fornecedor / Lancamento.</summary>
    [MaxLength(20)]
    public string Origem { get; set; } = string.Empty;

    /// <summary>Id do item de origem (FolhaItem, LancamentoFinanceiro, etc.).</summary>
    public int OrigemId { get; set; }

    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? CpfCnpj { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal Valor { get; set; }

    public long ValorCentavos { get; set; }

    [MaxLength(2)]
    public string? FormaLancamento { get; set; }

    public DateTime DataCnab { get; set; }

    /// <summary>VÁLIDO / CORRIGIDO / PENDENTE / INVÁLIDO.</summary>
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? SegmentosGerados { get; set; }

    [MaxLength(160)]
    public string? ArquivoDestino { get; set; }

    [MaxLength(1000)]
    public string? Observacoes { get; set; }

    /// <summary>StatusCnab do pagamento: CNAB_GERADO quando incluído com sucesso.</summary>
    [MaxLength(20)]
    public string StatusCnab { get; set; } = "PENDENTE";
}
