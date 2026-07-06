using MinhaAplicacaoBlazor.CnabBtg.Generation;

namespace MinhaAplicacaoBlazor.CnabBtg.Payments;

/// <summary>
/// Valida pagamentos já normalizados e os classifica. Não altera dados de negócio
/// (valor, banco, conta): apenas verifica e marca. O tamanho máximo da chave PIX
/// no Segmento B (Informação 10) é 35 posições.
/// </summary>
public static class PaymentValidator
{
    public const int TamanhoNomeFavorecido = 30;
    public const int TamanhoChavePix = 35;

    /// <summary>Valida a lista completa (inclui detecção de duplicidade CPF/CNPJ + valor + data).</summary>
    public static void ValidarLote(IReadOnlyList<NormalizedPayment> pagamentos, CnabGenerationOptions options)
    {
        foreach (var p in pagamentos)
            Validar(p, options);

        // Duplicidade: mesmo documento + valor + data.
        var grupos = pagamentos
            .Where(p => p.Status != StatusPagamentoCnab.Invalido)
            .GroupBy(p => $"{p.CpfCnpj}|{p.ValorCentavos}|{p.DataPagamento:yyyyMMdd}");

        foreach (var g in grupos.Where(g => g.Count() > 1))
        {
            foreach (var p in g)
                p.MarcarPendente("Possível duplicidade (mesmo CPF/CNPJ, valor e data).");
        }
    }

    public static void Validar(NormalizedPayment p, CnabGenerationOptions options)
    {
        // Nome
        if (string.IsNullOrWhiteSpace(p.Nome))
            p.MarcarInvalido("Nome do favorecido vazio.");
        else if (p.Nome.Length > TamanhoNomeFavorecido)
            p.RegistrarCorrecao($"Nome truncado para {TamanhoNomeFavorecido} caracteres.");

        // CPF/CNPJ
        if (string.IsNullOrWhiteSpace(p.CpfCnpj))
            p.MarcarInvalido("CPF/CNPJ ausente.");
        else if (!CnabText.CpfCnpjValido(p.CpfCnpj))
            p.MarcarInvalido($"CPF/CNPJ inválido: {p.CpfCnpj}.");

        // Valor
        if (p.Valor <= 0)
            p.MarcarInvalido($"Valor inválido (zero ou negativo): {p.Valor}.");

        // Data
        if (p.DataPagamento == default)
            p.MarcarInvalido("Data de pagamento vazia.");
        else if (p.DataPagamento.Date < options.GeradoEm.Date)
            p.MarcarInvalido($"Data de pagamento no passado: {p.DataPagamento:dd/MM/yyyy}.");

        // Forma
        if (!FormaLancamento.Suportada(p.Forma))
        {
            p.MarcarInvalido($"Forma de lançamento não suportada: '{p.Original.Forma}'.");
            return; // sem forma válida não dá para validar o restante
        }

        // Transferência bancária (crédito/poupança/TED/DOC)
        if (FormaLancamento.ExigeContaBancaria(p.Forma))
        {
            if (p.CodigoBanco.Length != 3)
                p.MarcarInvalido($"Banco favorecido deve ter 3 dígitos: '{p.Original.CodigoBanco}'.");
            if (string.IsNullOrWhiteSpace(p.Agencia))
                p.MarcarInvalido("Agência ausente para transferência bancária.");
            if (string.IsNullOrWhiteSpace(p.Conta))
                p.MarcarInvalido("Conta ausente para transferência bancária.");
            else if (string.IsNullOrWhiteSpace(p.ContaDv))
                p.MarcarInvalido("Dígito da conta ausente para transferência bancária.");
        }

        // PIX
        if (FormaLancamento.EhPix(p.Forma))
        {
            if (string.IsNullOrWhiteSpace(p.ChavePix))
                p.MarcarInvalido("Chave PIX ausente para pagamento PIX.");
            else if (p.ChavePix.Length > TamanhoChavePix)
                p.MarcarInvalido($"Chave PIX excede {TamanhoChavePix} caracteres (não truncar): {p.ChavePix.Length}.");

            if (string.IsNullOrWhiteSpace(p.FormaIniciacaoPix))
                p.MarcarPendente("Não foi possível inferir o tipo da chave PIX.");
        }
    }
}
