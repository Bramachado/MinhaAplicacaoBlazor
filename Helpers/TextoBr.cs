using System.Globalization;
using System.Text;

namespace MinhaAplicacaoBlazor.Helpers;

/// <summary>
/// Utilitários de formatação de texto no padrão brasileiro.
/// </summary>
public static class TextoBr
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>
    /// Conectores que permanecem em minúsculas quando não são a primeira palavra
    /// (ex.: "Maria da Silva", "Agência de Marketing de Eldorado").
    /// </summary>
    private static readonly HashSet<string> Conectores = new(StringComparer.OrdinalIgnoreCase)
    {
        "de", "da", "do", "das", "dos", "e", "di", "du", "del", "della", "van", "von", "y"
    };

    /// <summary>
    /// Converte um nome para "Title Case" no padrão brasileiro, mesmo que o texto
    /// venha todo em maiúsculas: normaliza para minúsculas e capitaliza cada
    /// palavra, mantendo os conectores (de, da, dos, e...) em minúsculo — exceto
    /// quando são a primeira palavra. Retorna o valor original se for nulo/vazio.
    /// </summary>
    public static string? ParaTitulo(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return valor;

        var palavras = valor.Trim().ToLower(Cultura)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < palavras.Length; i++)
        {
            if (i > 0 && Conectores.Contains(palavras[i]))
                continue;

            palavras[i] = Capitalizar(palavras[i]);
        }

        return string.Join(' ', palavras);
    }

    /// <summary>
    /// Capitaliza a primeira letra da palavra e também a letra após separadores
    /// internos (hífen, apóstrofo, ponto e parêntese) — ex.: "maria-clara" =>
    /// "Maria-Clara", "(pulse" => "(Pulse".
    /// </summary>
    private static string Capitalizar(string palavra)
    {
        var sb = new StringBuilder(palavra.Length);
        var capitalizarProxima = true;

        foreach (var ch in palavra)
        {
            if (capitalizarProxima && char.IsLetter(ch))
            {
                sb.Append(char.ToUpper(ch, Cultura));
                capitalizarProxima = false;
            }
            else
            {
                sb.Append(ch);
                if (ch is '-' or '\'' or '.' or '(' or '/')
                    capitalizarProxima = true;
            }
        }

        return sb.ToString();
    }
}
