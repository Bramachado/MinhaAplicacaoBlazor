using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>Resultado da geração: arquivos .rem, classificação dos pagamentos e totais.</summary>
public sealed class CnabGenerationResult
{
    public bool Bloqueado { get; set; }
    public string? MotivoBloqueio { get; set; }

    public int NsaInicial { get; set; }
    public int NsaFinal { get; set; }

    public List<CnabArquivoGerado> Arquivos { get; } = new();
    public List<NormalizedPayment> Pagamentos { get; set; } = new();

    public int TotalSelecionados => Pagamentos.Count;
    public int TotalValidos => Pagamentos.Count(p => p.Status == StatusPagamentoCnab.Valido);
    public int TotalCorrigidos => Pagamentos.Count(p => p.Status == StatusPagamentoCnab.CorrigidoAutomaticamente);
    public int TotalPendentes => Pagamentos.Count(p => p.Status == StatusPagamentoCnab.PendenteDeConfirmacao);
    public int TotalInvalidos => Pagamentos.Count(p => p.Status == StatusPagamentoCnab.Invalido);
    public int TotalGerados => Arquivos.Sum(a => a.Operacoes);

    public decimal ValorTotal => Arquivos.Sum(a => a.ValorTotal);

    public bool TodasLinhas240 => Arquivos.All(a => a.TodasLinhas240);
}

public sealed class CnabArquivoGerado
{
    public string FileName { get; set; } = string.Empty;
    public int Nsa { get; set; }
    public List<string> Linhas { get; } = new();
    public int Operacoes { get; set; }
    public decimal ValorTotal { get; set; }

    /// <summary>Quantidade de registros (linhas) do arquivo.</summary>
    public int QuantidadeRegistros => Linhas.Count;

    public int QuantidadeLotes { get; set; }

    public bool TodasLinhas240 => Linhas.All(l => l.Length == 240);

    /// <summary>Conteúdo final do .rem (linhas separadas por CRLF, sem BOM).</summary>
    public string Conteudo => string.Join("\r\n", Linhas) + "\r\n";
}
