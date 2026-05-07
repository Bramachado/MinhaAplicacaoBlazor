using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabTrailerArquivoBuilder
{
    public static string Build(ConfiguracaoCnab cfg, int totalRegistrosArquivo)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));     // 001-003
        sb.Append(Cnab240Formatter.Num("9999", 4));              // 004-007
        sb.Append('9');                                          // 008
        sb.Append(Cnab240Formatter.Brancos(9));                  // 009-017
        sb.Append(Cnab240Formatter.Num("1", 6));                 // 018-023 qtd lotes
        sb.Append(Cnab240Formatter.Num(totalRegistrosArquivo, 6)); // 024-029 qtd registros
        sb.Append(Cnab240Formatter.Num("0", 6));                 // 030-035 qtd contas conciliação
        sb.Append(Cnab240Formatter.Brancos(205));                // 036-240
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, "Trailer de Arquivo");
        return linha;
    }
}
