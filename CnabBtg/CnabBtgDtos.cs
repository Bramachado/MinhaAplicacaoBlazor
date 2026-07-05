using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg;

/// <summary>Pagamento candidato exibido/selecionado no popup (espelha o relatório).</summary>
public sealed class CnabBtgPagamentoDto
{
    /// <summary>Chave estável "Origem:OrigemId" para seleção.</summary>
    public string Chave => $"{Origem}:{OrigemId}";
    public string Origem { get; set; } = string.Empty;
    public int OrigemId { get; set; }
    public string? Unidade { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public decimal Valor { get; set; }
    public string? Forma { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePix { get; set; }
    public bool JaEmCnab { get; set; }
    public PaymentInput Input { get; set; } = new();
}

/// <summary>Perguntas do popup de geração.</summary>
public sealed class CnabBtgGerarRequest
{
    public int CompetenciaId { get; set; }
    public List<string> ChavesSelecionadas { get; set; } = new();

    public string EmpresaPagadora { get; set; } = "EDUNORTE";
    public DateTime DataPagamento { get; set; }
    public bool UsarNsaAutomatico { get; set; } = true;
    public int? NsaInicial { get; set; }
    public string NomeArquivo { get; set; } = "pagamentos";
    public string TipoPagamentoPrincipal { get; set; } = "PIX";
    public string Ambiente { get; set; } = "TESTE";
    public bool BloquearSeHouverInvalidos { get; set; }
    public bool SepararLotesPorForma { get; set; }
    public string? Convenio { get; set; }
    public string? GeradoPor { get; set; }
}

/// <summary>Resultado da geração devolvido ao popup.</summary>
public sealed class CnabBtgGeracaoResultadoDto
{
    public int BatchId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MotivoBloqueio { get; set; }
    public int NsaInicial { get; set; }
    public int NsaFinal { get; set; }
    public int TotalSelecionados { get; set; }
    public int TotalValidos { get; set; }
    public int TotalCorrigidos { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalInvalidos { get; set; }
    public decimal ValorTotal { get; set; }
    public bool TodasLinhas240 { get; set; }
    public List<CnabBtgArquivoResumoDto> Arquivos { get; set; } = new();
    public List<CnabBtgPagamentoResultadoDto> Pagamentos { get; set; } = new();
}

public sealed class CnabBtgArquivoResumoDto
{
    public string FileName { get; set; } = string.Empty;
    public int Nsa { get; set; }
    public int Operacoes { get; set; }
    public int QuantidadeRegistros { get; set; }
    public decimal ValorTotal { get; set; }
    public bool TodasLinhas240 { get; set; }
}

public sealed class CnabBtgPagamentoResultadoDto
{
    public int OrigemId { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ArquivoDestino { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>Detalhe de um lote (histórico): cabeçalho + arquivos + pagamentos.</summary>
public sealed class CnabBtgBatchDetalheDto
{
    public CnabBtgHistoricoDto Cabecalho { get; set; } = new();
    public int? CompetenciaId { get; set; }
    public string NomeBaseArquivo { get; set; } = string.Empty;
    public string? Convenio { get; set; }
    public int TotalValidos { get; set; }
    public int TotalCorrigidos { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalInvalidos { get; set; }
    public List<CnabBtgArquivoResumoDto> Arquivos { get; set; } = new();
    public List<CnabBtgBatchPagamentoDto> Pagamentos { get; set; } = new();
}

public sealed class CnabBtgBatchPagamentoDto
{
    public string Origem { get; set; } = string.Empty;
    public int OrigemId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public decimal Valor { get; set; }
    public string? Forma { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusCnab { get; set; } = string.Empty;
    public string? ArquivoDestino { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>Item de histórico de gerações.</summary>
public sealed class CnabBtgHistoricoDto
{
    public int BatchId { get; set; }
    public string EmpresaPagadora { get; set; } = string.Empty;
    public string Ambiente { get; set; } = string.Empty;
    public DateTime GeradoEm { get; set; }
    public string? GeradoPor { get; set; }
    public int NsaInicial { get; set; }
    public int NsaFinal { get; set; }
    public int QuantidadeArquivos { get; set; }
    public int TotalGerados { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
}
