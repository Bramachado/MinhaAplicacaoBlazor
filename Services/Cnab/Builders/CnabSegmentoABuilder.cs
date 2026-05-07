using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabSegmentoABuilder
{
    public static string Build(ConfiguracaoCnab cfg, FormaLancamentoCnab forma, RemessaCnabItem item, int seqLote)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));         // 001-003
        sb.Append(Cnab240Formatter.Num("1", 4));                     // 004-007 lote
        sb.Append('3');                                              // 008
        sb.Append(Cnab240Formatter.Num(seqLote, 5));                 // 009-013
        sb.Append('A');                                              // 014
        sb.Append(Cnab240Formatter.Num("0", 3));                     // 015-017 movimento 000
        sb.Append(Cnab240Formatter.Num(GeradorCnabPagamentoBtg.CamaraCentralizadora(forma.Codigo), 3)); // 018-020
        sb.Append(Cnab240Formatter.Num(item.BancoFavorecido, 3));    // 021-023
        sb.Append(Cnab240Formatter.Num(item.AgenciaFavorecido, 5));  // 024-028
        sb.Append(Cnab240Formatter.Alfa(item.AgenciaDVFavorecido, 1));// 029
        sb.Append(Cnab240Formatter.Num(item.ContaFavorecido, 12));   // 030-041
        sb.Append(Cnab240Formatter.Alfa(item.ContaDVFavorecido, 1)); // 042
        sb.Append(' ');                                              // 043
        sb.Append(Cnab240Formatter.Alfa(item.NomeFavorecido, 30));   // 044-073
        sb.Append(Cnab240Formatter.Alfa(item.SeuNumero, 20));        // 074-093
        sb.Append(Cnab240Formatter.Data(item.DataPagamento));        // 094-101
        sb.Append(Cnab240Formatter.Alfa("REA", 3));                  // 102-104 moeda
        sb.Append(Cnab240Formatter.Num("0", 15));                    // 105-119 ISPB/quant. moeda
        sb.Append(Cnab240Formatter.Valor(item.ValorPagamento, 15));  // 120-134
        sb.Append(Cnab240Formatter.Alfa(string.Empty, 15));          // 135-149 nº doc banco
        sb.Append(Cnab240Formatter.Num("0", 8));                     // 150-157 data efetiva
        sb.Append(Cnab240Formatter.Num("0", 15));                    // 158-172 valor efetivo
        sb.Append(Cnab240Formatter.Alfa(string.Empty, 40));          // 173-212 informação complementar
        sb.Append(Cnab240Formatter.Brancos(5));                      // 213-217
        sb.Append(Cnab240Formatter.Num("0", 2));                     // 218-219 finalidade TED
        sb.Append(Cnab240Formatter.Brancos(3));                      // 220-222
        sb.Append(Cnab240Formatter.Num("0", 2));                     // 223-224 aviso
        sb.Append(Cnab240Formatter.Brancos(6));                      // 225-230
        sb.Append(Cnab240Formatter.Brancos(10));                     // 231-240 ocorrência (retorno)
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, $"Segmento A item #{item.Id}");
        return linha;
    }
}
