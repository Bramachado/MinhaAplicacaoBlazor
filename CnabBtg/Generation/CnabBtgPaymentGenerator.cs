using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Gerador CNAB 240 do BTG (208), independente da origem dos dados: recebe uma
/// lista de <see cref="PaymentInput"/> e as opções, normaliza, valida, classifica
/// e monta um ou mais arquivos .rem (máx. 50 operações por arquivo, NSA por
/// arquivo). Um lote por forma de lançamento (o Header de Lote tem forma única).
/// </summary>
public sealed class CnabBtgPaymentGenerator
{
    public CnabGenerationResult Gerar(IReadOnlyList<PaymentInput> entradas, CnabGenerationOptions options)
    {
        var pagamentos = entradas
            .Select(e => PaymentNormalizer.Normalizar(e, options))
            .ToList();

        PaymentValidator.ValidarLote(pagamentos, options);

        var result = new CnabGenerationResult
        {
            Pagamentos = pagamentos,
            NsaInicial = options.NsaInicial,
            NsaFinal = options.NsaInicial
        };

        var haInvalidos = pagamentos.Any(p => p.Status == StatusPagamentoCnab.Invalido);
        if (options.TratamentoInvalidos == TratamentoInvalidos.Bloquear && haInvalidos)
        {
            result.Bloqueado = true;
            result.MotivoBloqueio =
                "Há pagamentos inválidos e a opção escolhida bloqueia a geração quando existem inválidos.";
            return result;
        }

        var geraveis = pagamentos.Where(p => p.Geravel).ToList();
        if (geraveis.Count == 0)
        {
            result.Bloqueado = true;
            result.MotivoBloqueio = "Nenhum pagamento válido para gerar.";
            return result;
        }

        var max = Math.Max(1, options.MaxOperacoesPorArquivo);
        var arquivosDePagamentos = ParticionarEmArquivos(geraveis, max, options.SepararLotesPorForma);

        for (var idx = 0; idx < arquivosDePagamentos.Count; idx++)
        {
            var nsa = options.NsaInicial + idx;
            var fileName = $"{options.NomeBaseArquivo}_{idx + 1:00}.rem";
            var arquivo = MontarArquivo(options, nsa, fileName, arquivosDePagamentos[idx]);
            result.Arquivos.Add(arquivo);
            result.NsaFinal = nsa;
        }

        return result;
    }

    /// <summary>Divide os pagamentos em arquivos de no máximo <paramref name="max"/> operações.</summary>
    private static List<List<NormalizedPayment>> ParticionarEmArquivos(
        List<NormalizedPayment> geraveis, int max, bool separarPorForma)
    {
        IEnumerable<NormalizedPayment> ordenados = separarPorForma
            ? geraveis.OrderBy(p => p.Forma).ThenBy(p => p.Nome)
            : geraveis;

        var arquivos = new List<List<NormalizedPayment>>();
        var atual = new List<NormalizedPayment>();

        foreach (var p in ordenados)
        {
            if (atual.Count == max
                || (separarPorForma && atual.Count > 0 && atual[0].Forma != p.Forma))
            {
                arquivos.Add(atual);
                atual = new List<NormalizedPayment>();
            }
            atual.Add(p);
        }
        if (atual.Count > 0)
            arquivos.Add(atual);

        return arquivos;
    }

    private static CnabArquivoGerado MontarArquivo(
        CnabGenerationOptions options, int nsa, string fileName, List<NormalizedPayment> pagamentos)
    {
        var arquivo = new CnabArquivoGerado { FileName = fileName, Nsa = nsa };
        arquivo.Linhas.Add(CnabRecordBuilders.HeaderArquivo(options, nsa));

        // Um lote por forma de lançamento (Header de Lote tem forma única).
        var lotesPorForma = pagamentos
            .GroupBy(p => p.Forma)
            .OrderBy(g => g.Key)
            .ToList();

        var numeroLote = 0;
        foreach (var grupo in lotesPorForma)
        {
            numeroLote++;
            var forma = grupo.Key;
            arquivo.Linhas.Add(CnabRecordBuilders.HeaderLote(options, numeroLote, forma));

            var seq = 0;
            decimal somaLote = 0m;
            foreach (var p in grupo)
            {
                p.ArquivoDestino = fileName;
                var seuNumero = $"CNAB{nsa:000000}{p.Id:000000}";

                seq++;
                arquivo.Linhas.Add(CnabRecordBuilders.SegmentoA(options, p, numeroLote, seq, seuNumero));
                seq++;
                arquivo.Linhas.Add(CnabRecordBuilders.SegmentoB(options, p, numeroLote, seq));

                somaLote += p.Valor;
                arquivo.Operacoes++;
                arquivo.ValorTotal += p.Valor;
            }

            // Registros do lote = header + detalhes + trailer.
            var qtdRegistrosLote = 2 + grupo.Count() * 2;
            arquivo.Linhas.Add(CnabRecordBuilders.TrailerLote(options, numeroLote, qtdRegistrosLote, somaLote));
        }

        arquivo.QuantidadeLotes = numeroLote;
        // Trailer de arquivo: total de lotes e total de registros (todas as linhas + o próprio trailer).
        var totalRegistros = arquivo.Linhas.Count + 1;
        arquivo.Linhas.Add(CnabRecordBuilders.TrailerArquivo(options, numeroLote, totalRegistros));

        return arquivo;
    }
}
