using System.Globalization;
using System.Text;
using System.Text.Json;
using MinhaAplicacaoBlazor.CnabBtg.Generation;
using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg.Audit;

/// <summary>
/// Monta o <see cref="CnabAuditReport"/> a partir do resultado da geração e o
/// serializa em JSON e CSV.
/// </summary>
public static class CnabAuditWriter
{
    private static readonly JsonSerializerOptions JsonOpcoes = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static CnabAuditReport Montar(CnabGenerationResult resultado, CnabGenerationOptions options)
    {
        var report = new CnabAuditReport
        {
            DataHoraGeracao = options.GeradoEm.ToString("yyyy-MM-dd HH:mm:ss"),
            Usuario = options.GeradoPor,
            Ambiente = options.Ambiente.ToString().ToUpperInvariant(),
            EmpresaPagadora = options.Empresa.Codigo,
            Banco = options.Empresa.Banco,
            Agencia = $"{options.Empresa.Agencia}{(string.IsNullOrEmpty(options.Empresa.AgenciaDv) ? "" : "-" + options.Empresa.AgenciaDv)}",
            Conta = $"{options.Empresa.Conta}{(string.IsNullOrEmpty(options.Empresa.ContaDv) ? "" : "-" + options.Empresa.ContaDv)}",
            Convenio = options.ConvenioEfetivo,
            NsaInicial = resultado.NsaInicial,
            NsaFinal = resultado.NsaFinal,
            TotalSelecionados = resultado.TotalSelecionados,
            TotalValidos = resultado.TotalValidos,
            TotalCorrigidos = resultado.TotalCorrigidos,
            TotalPendentes = resultado.TotalPendentes,
            TotalInvalidos = resultado.TotalInvalidos,
            TotalGerados = resultado.TotalGerados,
            ValorTotalGeral = resultado.ValorTotal,
            Bloqueado = resultado.Bloqueado,
            MotivoBloqueio = resultado.MotivoBloqueio
        };

        report.Arquivos = resultado.Arquivos.Select(a => new CnabAuditArquivo
        {
            Arquivo = a.FileName,
            Nsa = a.Nsa,
            Operacoes = a.Operacoes,
            QuantidadeRegistros = a.QuantidadeRegistros,
            QuantidadeLotes = a.QuantidadeLotes,
            ValorTotal = a.ValorTotal,
            TodasLinhas240 = a.TodasLinhas240
        }).ToList();

        report.Pagamentos = resultado.Pagamentos.Select(p => new CnabAuditRow
        {
            PagamentoId = p.Id,
            Origem = p.Origem,
            NomeOriginal = p.Original.NomeTitular ?? p.Original.Nome,
            NomeNormalizado = p.Nome,
            CpfCnpjOriginal = p.Original.CpfCnpj,
            CpfCnpjNormalizado = p.CpfCnpj,
            ValorOriginal = p.Original.Valor,
            ValorCentavos = p.ValorCentavos,
            DataOriginal = p.Original.DataPagamento?.ToString("dd/MM/yyyy"),
            DataCnab = p.DataPagamento.ToString("ddMMyyyy"),
            FormaOriginal = p.Original.Forma,
            FormaLancamento = p.Forma,
            ChavePixOriginal = p.Original.ChavePix,
            TipoChavePix = p.FormaIniciacaoPix,
            SegmentosGerados = p.Geravel ? p.SegmentosGerados : string.Empty,
            ArquivoDestino = p.ArquivoDestino,
            Status = p.Status.ToString(),
            Correcoes = string.Join(" | ", p.Correcoes),
            Observacoes = string.Join(" | ", p.Erros.Concat(p.Avisos))
        }).ToList();

        return report;
    }

    public static string ParaJson(CnabAuditReport report) =>
        JsonSerializer.Serialize(report, JsonOpcoes);

    public static string ParaCsv(CnabAuditReport report)
    {
        var ci = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(";", new[]
        {
            "PagamentoId", "Origem", "NomeOriginal", "NomeNormalizado",
            "CpfCnpjOriginal", "CpfCnpjNormalizado", "ValorOriginal", "ValorCentavos",
            "DataOriginal", "DataCnab", "FormaOriginal", "FormaLancamento",
            "ChavePixOriginal", "TipoChavePix", "SegmentosGerados", "ArquivoDestino",
            "Status", "Correcoes", "Observacoes"
        }));

        foreach (var r in report.Pagamentos)
        {
            sb.AppendLine(string.Join(";", new[]
            {
                C(r.PagamentoId.ToString(ci)),
                C(r.Origem),
                C(r.NomeOriginal),
                C(r.NomeNormalizado),
                C(r.CpfCnpjOriginal),
                C(r.CpfCnpjNormalizado),
                C(r.ValorOriginal.ToString("0.00", ci)),
                C(r.ValorCentavos.ToString(ci)),
                C(r.DataOriginal),
                C(r.DataCnab),
                C(r.FormaOriginal),
                C(r.FormaLancamento),
                C(r.ChavePixOriginal),
                C(r.TipoChavePix),
                C(r.SegmentosGerados),
                C(r.ArquivoDestino),
                C(r.Status),
                C(r.Correcoes),
                C(r.Observacoes)
            }));
        }

        return sb.ToString();
    }

    /// <summary>Escapa um campo CSV (delimitador ';', aspas duplas).</summary>
    private static string C(string? valor)
    {
        var v = valor ?? string.Empty;
        if (v.Contains(';') || v.Contains('"') || v.Contains('\n') || v.Contains('\r'))
            return "\"" + v.Replace("\"", "\"\"") + "\"";
        return v;
    }
}
