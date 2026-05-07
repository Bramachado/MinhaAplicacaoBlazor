using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public class GeradorCnabPagamentoBtg
{
    private const string Crlf = "\r\n";

    public string Gerar(ConfiguracaoCnab cfg, RemessaCnab remessa, FormaLancamentoCnab forma)
    {
        ArgumentNullException.ThrowIfNull(cfg);
        ArgumentNullException.ThrowIfNull(remessa);
        ArgumentNullException.ThrowIfNull(forma);

        if (!FormaSuportada(forma.Codigo))
            throw new NotSupportedException(
                $"Forma de lançamento {forma.Codigo} ({forma.Nome}) ainda não está implementada para geração CNAB.");

        var sb = new StringBuilder();

        sb.Append(CnabHeaderArquivoBuilder.Build(cfg, remessa));
        sb.Append(Crlf);

        sb.Append(CnabHeaderLoteBuilder.Build(cfg, remessa, forma));
        sb.Append(Crlf);

        var seqLote = 1;
        decimal somaLote = 0m;
        var qtdRegistrosLote = 2; // header + trailer do lote

        foreach (var item in remessa.Itens.OrderBy(i => i.Id))
        {
            sb.Append(CnabSegmentoABuilder.Build(cfg, forma, item, seqLote));
            sb.Append(Crlf);
            seqLote++;
            qtdRegistrosLote++;

            sb.Append(CnabSegmentoBBuilder.Build(cfg, forma, item, seqLote));
            sb.Append(Crlf);
            seqLote++;
            qtdRegistrosLote++;

            somaLote += item.ValorPagamento;
        }

        sb.Append(CnabTrailerLoteBuilder.Build(cfg, qtdRegistrosLote, somaLote));
        sb.Append(Crlf);

        var totalRegistros = qtdRegistrosLote + 2; // + header e trailer de arquivo
        sb.Append(CnabTrailerArquivoBuilder.Build(cfg, totalRegistros));
        sb.Append(Crlf);

        return sb.ToString();
    }

    public static bool FormaSuportada(string codigo) =>
        codigo is "01" or "03" or "41" or "43" or "45";

    public static string CamaraCentralizadora(string codigoForma) => codigoForma switch
    {
        "01" => "000",
        "03" => "700",
        "41" => "018",
        "43" => "018",
        "45" => "988",
        _ => "000"
    };
}
