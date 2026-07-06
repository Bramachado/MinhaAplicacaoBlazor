using MinhaAplicacaoBlazor.CnabBtg.Generation;

namespace MinhaAplicacaoBlazor.CnabBtg.Payments;

/// <summary>
/// Pagamento após normalização/validação: dados limpos prontos para o gerador,
/// mais a classificação (VÁLIDO / CORRIGIDO / PENDENTE / INVÁLIDO), as correções
/// aplicadas e um "retrato" dos dados originais para a auditoria.
/// </summary>
public sealed class NormalizedPayment
{
    public PaymentInput Original { get; init; } = new();

    public int Id => Original.Id;
    public string Origem => Original.Origem;

    // ---- Dados normalizados ----
    public string Nome { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public string TipoInscricao { get; set; } = "1"; // 1 CPF, 2 CNPJ
    public decimal Valor { get; set; }
    public string Forma { get; set; } = string.Empty;
    public string? ChavePix { get; set; }
    public string? FormaIniciacaoPix { get; set; } // 001..005
    public string CodigoBanco { get; set; } = string.Empty;
    public string Agencia { get; set; } = string.Empty;
    public string AgenciaDv { get; set; } = string.Empty;
    public string Conta { get; set; } = string.Empty;
    public string ContaDv { get; set; } = string.Empty;
    public DateTime DataPagamento { get; set; }

    // ---- Classificação ----
    public StatusPagamentoCnab Status { get; set; } = StatusPagamentoCnab.Valido;
    public List<string> Correcoes { get; } = new();
    public List<string> Erros { get; } = new();
    public List<string> Avisos { get; } = new();

    /// <summary>Segmentos que serão gerados (ex.: "A", "B").</summary>
    public string SegmentosGerados { get; set; } = "A,B";

    /// <summary>Arquivo .rem de destino (preenchido na fase de geração).</summary>
    public string? ArquivoDestino { get; set; }

    public long ValorCentavos => (long)Math.Round(Valor * 100m, MidpointRounding.AwayFromZero);

    public bool Geravel => Status is StatusPagamentoCnab.Valido or StatusPagamentoCnab.CorrigidoAutomaticamente;

    public void MarcarInvalido(string erro)
    {
        Erros.Add(erro);
        Status = StatusPagamentoCnab.Invalido;
    }

    public void MarcarPendente(string aviso)
    {
        Avisos.Add(aviso);
        if (Status != StatusPagamentoCnab.Invalido)
            Status = StatusPagamentoCnab.PendenteDeConfirmacao;
    }

    public void RegistrarCorrecao(string correcao)
    {
        Correcoes.Add(correcao);
        if (Status == StatusPagamentoCnab.Valido)
            Status = StatusPagamentoCnab.CorrigidoAutomaticamente;
    }
}
