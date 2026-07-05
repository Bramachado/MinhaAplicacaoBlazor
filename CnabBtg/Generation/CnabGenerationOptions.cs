namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Parâmetros de uma geração de CNAB (as "perguntas" respondidas pelo usuário
/// no popup). O gerador é puro: recebe estas opções + a lista de pagamentos.
/// </summary>
public sealed class CnabGenerationOptions
{
    public EmpresaPagadora Empresa { get; set; } = new();

    /// <summary>Data de pagamento aplicada aos itens que não trouxerem data própria.</summary>
    public DateTime DataPagamento { get; set; }

    /// <summary>NSA do primeiro arquivo. Cada arquivo adicional incrementa +1.</summary>
    public int NsaInicial { get; set; } = 1;

    /// <summary>Nome base do(s) arquivo(s), sem extensão. Ex.: "pagamentos_2026_07_10".</summary>
    public string NomeBaseArquivo { get; set; } = "cnab";

    /// <summary>Forma principal (usada como fallback quando o item não define forma).</summary>
    public string? TipoPagamentoPrincipal { get; set; }

    public AmbienteCnab Ambiente { get; set; } = AmbienteCnab.Teste;

    public TratamentoInvalidos TratamentoInvalidos { get; set; } = TratamentoInvalidos.Remover;

    /// <summary>Quando true, gera um lote por forma de lançamento; senão, lote único por arquivo.</summary>
    public bool SepararLotesPorForma { get; set; }

    /// <summary>Convênio diferente do padrão da empresa (opcional).</summary>
    public string? ConvenioOverride { get; set; }

    /// <summary>Máximo de operações por arquivo .rem (regra de negócio: 50).</summary>
    public int MaxOperacoesPorArquivo { get; set; } = 50;

    /// <summary>Data/hora da geração (injetada — o gerador não chama DateTime.Now).</summary>
    public DateTime GeradoEm { get; set; }

    public string? GeradoPor { get; set; }

    public string ConvenioEfetivo =>
        string.IsNullOrWhiteSpace(ConvenioOverride) ? Empresa.Convenio : ConvenioOverride!;
}
