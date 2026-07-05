namespace MinhaAplicacaoBlazor.CnabBtg.Audit;

/// <summary>Linha de auditoria de um pagamento: original vs normalizado, status, destino.</summary>
public sealed class CnabAuditRow
{
    public int PagamentoId { get; set; }
    public string Origem { get; set; } = string.Empty;

    public string NomeOriginal { get; set; } = string.Empty;
    public string NomeNormalizado { get; set; } = string.Empty;

    public string? CpfCnpjOriginal { get; set; }
    public string CpfCnpjNormalizado { get; set; } = string.Empty;

    public decimal ValorOriginal { get; set; }
    public long ValorCentavos { get; set; }

    public string? DataOriginal { get; set; }
    public string DataCnab { get; set; } = string.Empty;

    public string? FormaOriginal { get; set; }
    public string FormaLancamento { get; set; } = string.Empty;

    public string? ChavePixOriginal { get; set; }
    public string? TipoChavePix { get; set; }

    public string SegmentosGerados { get; set; } = string.Empty;
    public string? ArquivoDestino { get; set; }

    public string Status { get; set; } = string.Empty;
    public string Correcoes { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}
