using MinhaAplicacaoBlazor.CnabBtg.Generation;

namespace MinhaAplicacaoBlazor.CnabBtg.Payments;

/// <summary>
/// Aplica as correções automáticas permitidas (limpar CPF/CNPJ, caixa alta,
/// separar conta/dígito, converter data/forma, inferir tipo de chave PIX...).
/// Nunca inventa dados ausentes — apenas normaliza o que existe.
/// </summary>
public static class PaymentNormalizer
{
    public static NormalizedPayment Normalizar(PaymentInput input, CnabGenerationOptions options)
    {
        var p = new NormalizedPayment { Original = input };

        // Nome
        var nome = CnabText.NormalizarAlfa(input.NomeTitular ?? input.Nome);
        if (!string.Equals(nome, (input.NomeTitular ?? input.Nome)?.Trim(), StringComparison.Ordinal))
            p.RegistrarCorrecao("Nome normalizado (acentos/caixa/espaços).");
        p.Nome = nome;

        // CPF/CNPJ
        var doc = CnabText.ApenasDigitos(input.CpfCnpj);
        if (!string.IsNullOrEmpty(input.CpfCnpj) && doc != input.CpfCnpj)
            p.RegistrarCorrecao("CPF/CNPJ: removidos pontos/traços/barras.");
        p.CpfCnpj = doc;
        p.TipoInscricao = InferirTipoInscricao(input.TipoPessoa, doc);

        // Valor
        p.Valor = input.Valor;

        // Forma
        var forma = FormaLancamento.Normalizar(input.Forma, options.TipoPagamentoPrincipal);
        p.Forma = forma ?? string.Empty;

        // Data
        p.DataPagamento = input.DataPagamento ?? options.DataPagamento;

        // Dados bancários — separar conta/dígito e agência/dígito quando colados
        var (agencia, agDv) = CnabText.SepararDigito(input.Agencia);
        p.Agencia = agencia;
        p.AgenciaDv = agDv;

        string contaNum, contaDv;
        if (!string.IsNullOrWhiteSpace(input.DigitoConta))
        {
            contaNum = CnabText.ApenasDigitos(input.Conta);
            contaDv = CnabText.ApenasDigitos(input.DigitoConta);
        }
        else
        {
            (contaNum, contaDv) = CnabText.SepararDigito(input.Conta, separarUltimoSeColado: true);
            if (!string.IsNullOrEmpty(contaDv))
                p.RegistrarCorrecao("Conta e dígito separados automaticamente.");
        }
        p.Conta = contaNum;
        p.ContaDv = contaDv;

        p.CodigoBanco = CnabText.ApenasDigitos(input.CodigoBanco);

        // PIX
        if (FormaLancamento.EhPix(p.Forma))
        {
            p.ChavePix = (input.ChavePix ?? string.Empty).Trim();
            p.FormaIniciacaoPix = MapearTipoChaveExplicito(input.TipoChavePix)
                                  ?? InferirFormaIniciacaoPix(p.ChavePix);
            p.SegmentosGerados = "A,B";
        }

        return p;
    }

    private static string InferirTipoInscricao(string? tipoPessoa, string doc)
    {
        if (!string.IsNullOrWhiteSpace(tipoPessoa))
        {
            var t = tipoPessoa.Trim().ToUpperInvariant();
            if (t.StartsWith("J")) return "2"; // Juridica
            if (t.StartsWith("F")) return "1"; // Fisica
        }
        return doc.Length == 14 ? "2" : "1";
    }

    /// <summary>Mapeia o tipo de chave PIX informado explicitamente para o código (ou null se "Nenhuma"/vazio).</summary>
    private static string? MapearTipoChaveExplicito(string? tipo)
    {
        var t = (tipo ?? string.Empty).Trim().ToUpperInvariant();
        return t switch
        {
            "TELEFONE" => "001",
            "EMAIL" or "E-MAIL" => "002",
            "CPFCNPJ" or "CPF" or "CNPJ" => "003",
            "ALEATORIA" or "ALEATÓRIA" or "EVP" => "004",
            _ => null
        };
    }

    /// <summary>
    /// Infere a forma de iniciação PIX pela chave: telefone 001, e-mail 002,
    /// CPF/CNPJ 003, aleatória 004. Dados bancários (005) não têm chave.
    /// </summary>
    public static string InferirFormaIniciacaoPix(string? chave)
    {
        if (string.IsNullOrWhiteSpace(chave))
            return "005"; // sem chave: dados bancários

        var c = chave.Trim();

        if (c.Contains('@'))
            return "002"; // e-mail

        var digitos = CnabText.ApenasDigitos(c);

        // Chave aleatória (EVP): UUID de 32 hex / 36 com hífens.
        var semHifen = c.Replace("-", string.Empty);
        if (semHifen.Length == 32 && semHifen.All(Uri.IsHexDigit))
            return "004";

        if (digitos.Length == 11 && (c.StartsWith("+55") || digitos.StartsWith("55") || c.StartsWith('(')))
            return "001"; // telefone
        if ((c.StartsWith("+") || digitos.Length is 12 or 13) && digitos.Length >= 12)
            return "001"; // telefone com DDI

        if (digitos.Length is 11 or 14 && digitos == c)
            return "003"; // CPF/CNPJ

        return "004"; // fallback: trata como aleatória
    }
}
