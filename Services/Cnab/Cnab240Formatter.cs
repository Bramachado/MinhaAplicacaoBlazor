using System.Globalization;
using System.Text;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class Cnab240Formatter
{
    public const int TamanhoLinha = 240;

    public static string Alfa(string? valor, int tamanho)
    {
        var s = RemoverAcentos(valor ?? string.Empty).ToUpperInvariant();
        s = new string(s.Where(c => c >= 32 && c <= 126).ToArray());
        if (s.Length > tamanho) s = s[..tamanho];
        return s.PadRight(tamanho, ' ');
    }

    public static string Num(string? valor, int tamanho)
    {
        var s = new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        if (s.Length > tamanho) s = s[^tamanho..];
        return s.PadLeft(tamanho, '0');
    }

    public static string Num(int valor, int tamanho) => Num(valor.ToString(), tamanho);
    public static string Num(long valor, int tamanho) => Num(valor.ToString(), tamanho);

    public static string Valor(decimal valor, int tamanho)
    {
        var centavos = Math.Round(valor * 100m, 0, MidpointRounding.AwayFromZero);
        var s = ((long)centavos).ToString(CultureInfo.InvariantCulture);
        return s.PadLeft(tamanho, '0');
    }

    public static string Data(DateTime data) => data.ToString("ddMMyyyy");
    public static string DataOuZeros(DateTime? data) => data?.ToString("ddMMyyyy") ?? new string('0', 8);
    public static string Hora(DateTime data) => data.ToString("HHmmss");

    public static string Brancos(int tamanho) => new(' ', tamanho);
    public static string Zeros(int tamanho) => new('0', tamanho);

    public static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return string.Empty;
        var normalized = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static void ValidarLinha240(string linha, string contexto)
    {
        if (linha is null) throw new InvalidOperationException($"Linha CNAB nula em {contexto}.");
        if (linha.Length != TamanhoLinha)
            throw new InvalidOperationException(
                $"Linha CNAB inválida em {contexto}: tamanho {linha.Length}, esperado {TamanhoLinha}.");
    }
}
