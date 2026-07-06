namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Formas de lançamento FEBRABAN suportadas por este gerador (Segmentos A/B).
/// Códigos não listados aqui não são gerados sem módulo específico.
/// </summary>
public static class FormaLancamento
{
    public const string CreditoContaCorrente = "01";
    public const string DocTed = "03";
    public const string CreditoPoupanca = "05";
    public const string TedOutraTitularidade = "41";
    public const string TedMesmaTitularidade = "43";
    public const string PixTransferencia = "45";

    private static readonly HashSet<string> Suportadas = new()
    {
        CreditoContaCorrente, DocTed, CreditoPoupanca,
        TedOutraTitularidade, TedMesmaTitularidade, PixTransferencia
    };

    public static bool Suportada(string? codigo) => codigo is not null && Suportadas.Contains(codigo);

    public static bool EhPix(string? codigo) => codigo == PixTransferencia;

    public static bool EhPoupanca(string? codigo) => codigo == CreditoPoupanca;

    /// <summary>Exige dados bancários (banco/agência/conta/dígito).</summary>
    public static bool ExigeContaBancaria(string? codigo) =>
        codigo is CreditoContaCorrente or DocTed or CreditoPoupanca
               or TedOutraTitularidade or TedMesmaTitularidade;

    /// <summary>
    /// Normaliza uma "forma" possivelmente textual (PIX, TED, CREDITO, POUPANCA)
    /// para o código FEBRABAN. Retorna null quando não reconhece — o validador
    /// então rejeita (nunca inventa forma).
    /// </summary>
    public static string? Normalizar(string? forma, string? tipoPagamentoPrincipal = null)
    {
        if (string.IsNullOrWhiteSpace(forma))
            forma = tipoPagamentoPrincipal;

        var f = (forma ?? string.Empty).Trim().ToUpperInvariant();

        return f switch
        {
            "01" or "03" or "05" or "41" or "43" or "45" => f,
            "PIX" => PixTransferencia,
            "TED" => TedOutraTitularidade,
            "CREDITO" or "CRÉDITO" or "CREDITO EM CONTA" or "CONTA" or "CORRENTE" => CreditoContaCorrente,
            "POUPANCA" or "POUPANÇA" => CreditoPoupanca,
            _ => null
        };
    }

    /// <summary>Câmara de compensação por forma (posições do Segmento A).</summary>
    public static string Camara(string codigo) => codigo switch
    {
        DocTed => "700",           // DOC
        TedOutraTitularidade => "018",
        TedMesmaTitularidade => "018",
        PixTransferencia => "009",
        _ => "000"                 // crédito em conta / poupança no próprio banco
    };
}

/// <summary>Classificação de cada pagamento após normalização/validação.</summary>
public enum StatusPagamentoCnab
{
    Valido,
    CorrigidoAutomaticamente,
    PendenteDeConfirmacao,
    Invalido
}

/// <summary>Ambiente de geração.</summary>
public enum AmbienteCnab
{
    Teste,
    Producao
}

/// <summary>O que fazer com pagamentos inválidos na geração.</summary>
public enum TratamentoInvalidos
{
    /// <summary>Remove os inválidos e gera apenas com os válidos.</summary>
    Remover,
    /// <summary>Bloqueia toda a geração se houver qualquer inválido.</summary>
    Bloquear
}
