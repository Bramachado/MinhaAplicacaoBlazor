using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabTrailerLoteBuilder
{
    public static string Build(ConfiguracaoCnab cfg, int qtdRegistrosLote, decimal somatorioValores)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));     // 001-003
        sb.Append(Cnab240Formatter.Num("1", 4));                 // 004-007 lote
        sb.Append('5');                                          // 008
        sb.Append(Cnab240Formatter.Brancos(9));                  // 009-017
        sb.Append(Cnab240Formatter.Num(qtdRegistrosLote, 6));    // 018-023
        sb.Append(Cnab240Formatter.Valor(somatorioValores, 18)); // 024-041 somatório valores
        sb.Append(Cnab240Formatter.Num("0", 18));                // 042-059 somatório quant. moeda
        sb.Append(Cnab240Formatter.Num("0", 6));                 // 060-065 nº aviso débito
        sb.Append(Cnab240Formatter.Brancos(165));                // 066-230
        sb.Append(Cnab240Formatter.Brancos(10));                 // 231-240 ocorrência
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, "Trailer de Lote");
        return linha;
    }
}
