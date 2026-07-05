using System.Globalization;
using System.Text;

namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Utilitários de texto para o CNAB FEBRABAN 240: normalização de nomes,
/// extração de dígitos, remoção de acentos e quebras de linha.
/// Não formata posições (isso é responsabilidade do <see cref="Cnab240LineBuilder"/>) —
/// apenas limpa/normaliza valores de entrada.
/// </summary>
public static class CnabText
{
    /// <summary>Remove acentos/diacríticos, mantendo as letras base.</summary>
    public static string RemoverAcentos(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        var normalizado = valor.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Normaliza um nome/alfanumérico para o CNAB: remove acentos e quebras de linha,
    /// converte para CAIXA ALTA, colapsa espaços duplicados e mantém apenas
    /// caracteres ASCII imprimíveis (32–126).
    /// </summary>
    public static string NormalizarAlfa(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var semAcento = RemoverAcentos(valor)
            .Replace('\r', ' ')
            .Replace('\n', ' ')
            .Replace('\t', ' ')
            .ToUpperInvariant();

        var sb = new StringBuilder(semAcento.Length);
        foreach (var c in semAcento)
        {
            // Mantém apenas ASCII imprimível; demais viram espaço.
            sb.Append(c is >= ' ' and <= '~' ? c : ' ');
        }

        return ColapsarEspacos(sb.ToString()).Trim();
    }

    /// <summary>Mantém apenas os dígitos de uma string (CPF/CNPJ, conta, agência, valor).</summary>
    public static string ApenasDigitos(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        var sb = new StringBuilder(valor.Length);
        foreach (var c in valor)
        {
            if (char.IsDigit(c))
                sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>Colapsa sequências de espaços em um único espaço.</summary>
    public static string ColapsarEspacos(string valor)
    {
        var sb = new StringBuilder(valor.Length);
        var espacoAnterior = false;
        foreach (var c in valor)
        {
            if (c == ' ')
            {
                if (!espacoAnterior)
                    sb.Append(' ');
                espacoAnterior = true;
            }
            else
            {
                sb.Append(c);
                espacoAnterior = false;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Separa a parte numérica de um dígito verificador quando vierem juntos
    /// (ex.: "829434-3" → ("829434","3"); "8294343" com <paramref name="separarUltimo"/>
    /// true → ("829434","3")). Retorna dígito vazio quando não há como inferir.
    /// </summary>
    public static (string Numero, string Digito) SepararDigito(string? valor, bool separarUltimoSeColado = false)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return (string.Empty, string.Empty);

        var v = valor.Trim();
        var sep = v.LastIndexOfAny(new[] { '-', '.', '/' });
        if (sep >= 0)
        {
            var numero = ApenasDigitos(v[..sep]);
            var digito = ApenasDigitos(v[(sep + 1)..]);
            return (numero, digito);
        }

        var digitos = ApenasDigitos(v);
        if (separarUltimoSeColado && digitos.Length > 1)
            return (digitos[..^1], digitos[^1..]);

        return (digitos, string.Empty);
    }

    /// <summary>Valida CPF (11) ou CNPJ (14) pelos dígitos verificadores.</summary>
    public static bool CpfCnpjValido(string? valor)
    {
        var d = ApenasDigitos(valor);
        return d.Length switch
        {
            11 => CpfValido(d),
            14 => CnpjValido(d),
            _ => false
        };
    }

    private static bool CpfValido(string cpf)
    {
        if (new string(cpf[0], 11) == cpf)
            return false;

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);
        var d1 = Resto11(soma);
        if (d1 != cpf[9] - '0')
            return false;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);
        var d2 = Resto11(soma);
        return d2 == cpf[10] - '0';
    }

    private static bool CnpjValido(string cnpj)
    {
        if (new string(cnpj[0], 14) == cnpj)
            return false;

        int[] peso1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] peso2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;
        for (var i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * peso1[i];
        var d1 = Resto11(soma);
        if (d1 != cnpj[12] - '0')
            return false;

        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * peso2[i];
        var d2 = Resto11(soma);
        return d2 == cnpj[13] - '0';
    }

    private static int Resto11(int soma)
    {
        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
