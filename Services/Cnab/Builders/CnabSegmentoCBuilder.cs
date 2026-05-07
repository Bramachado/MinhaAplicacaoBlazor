using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabSegmentoCBuilder
{
    /// <summary>
    /// Segmento C — informações complementares opcionais (ex.: rateio).
    /// Não usado no fluxo básico TED/PIX. Deixado preparado para evolução.
    /// </summary>
    public static string Build(ConfiguracaoCnab cfg, RemessaCnabItem item, int seqLote)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));
        sb.Append(Cnab240Formatter.Num("1", 4));
        sb.Append('3');
        sb.Append(Cnab240Formatter.Num(seqLote, 5));
        sb.Append('C');
        sb.Append(Cnab240Formatter.Brancos(227));
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, "Segmento C");
        return linha;
    }
}
