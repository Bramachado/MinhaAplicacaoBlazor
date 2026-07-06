using MinhaAplicacaoBlazor.CnabBtg.Generation;
using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg;

/// <summary>
/// Resumo da competência para a tela "Gerenciar CNAB": todos os pagamentos
/// (transferências E boletos), separados e totalizados para conferência antes da
/// geração. Boletos entram nos totais da competência, mas NÃO no arquivo .rem.
/// </summary>
public sealed class CnabGerenciarResumoDto
{
    public int CompetenciaId { get; set; }
    public string CompetenciaTexto { get; set; } = string.Empty;

    // Totais da competência (todos os pagamentos).
    public int TotalPagamentos { get; set; }
    public decimal ValorTotalCompetencia { get; set; }

    public int TotalTransferencias { get; set; }
    public decimal ValorTransferencias { get; set; }

    // Transferências separadas por tipo (PIX = forma 45; TED = demais transferências bancárias).
    public int TotalPix { get; set; }
    public decimal ValorPix { get; set; }

    public int TotalTed { get; set; }
    public decimal ValorTed { get; set; }

    public int TotalBoletos { get; set; }
    public decimal ValorBoletos { get; set; }

    // Totais dependentes da seleção do usuário (recalculados na tela).
    public int TotalSelecionados { get; set; }
    public decimal ValorSelecionado { get; set; }

    public int TotalValidos { get; set; }
    public decimal ValorValidos { get; set; }

    public int TotalInvalidos { get; set; }
    public decimal ValorInvalidos { get; set; }

    public int TotalJaGerados { get; set; }
    public decimal ValorJaGerados { get; set; }

    /// <summary>Diferença entre o total selecionado e o que será efetivamente gerado (transferências válidas).</summary>
    public decimal DiferencaSelecionadoGerado { get; set; }

    public List<CnabGerenciarPagamentoDto> Pagamentos { get; set; } = new();
}

/// <summary>Um pagamento na conferência da tela "Gerenciar CNAB".</summary>
public sealed class CnabGerenciarPagamentoDto
{
    /// <summary>Chave estável "Origem:OrigemId".</summary>
    public string Chave => $"{Origem}:{OrigemId}";

    public string Origem { get; set; } = string.Empty;
    public int OrigemId { get; set; }
    public string? Unidade { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public decimal Valor { get; set; }

    /// <summary>"Transferencia" ou "Boleto".</summary>
    public string TipoOperacaoCnab { get; set; } = string.Empty;

    /// <summary>Forma FEBRABAN inferida (01, 05, 41, 43, 45) — só para transferências.</summary>
    public string? Forma { get; set; }

    public string? BancoCadastro { get; set; }
    public string? BancoFavorecido { get; set; }
    public string? CodigoBanco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? DigitoConta { get; set; }
    public string? ChavePix { get; set; }

    public bool JaEmCnab { get; set; }

    /// <summary>True quando o pagamento pode ser marcado para geração (transferência, não já gerada).</summary>
    public bool Selecionavel { get; set; }

    /// <summary>Valido, Corrigido, Pendente, Invalido, Boleto, JaGerado.</summary>
    public string StatusValidacao { get; set; } = string.Empty;

    public List<string> Alertas { get; set; } = new();

    public bool EhBoleto => string.Equals(TipoOperacaoCnab, "Boleto", StringComparison.OrdinalIgnoreCase);
    public bool EhTransferencia => string.Equals(TipoOperacaoCnab, "Transferencia", StringComparison.OrdinalIgnoreCase);
    public bool Invalido => string.Equals(StatusValidacao, "Invalido", StringComparison.OrdinalIgnoreCase);

    /// <summary>"PIX" (forma 45) ou "TED" (demais transferências bancárias). Null para boletos.</summary>
    public string? TipoTransferencia => !EhTransferencia
        ? null
        : (Forma == FormaLancamento.PixTransferencia ? "PIX" : "TED");

    /// <summary>
    /// Entra no .rem: transferência válida e selecionável. Se está "já em CNAB",
    /// só é selecionável quando o modo "permitir regerar" estiver ligado (o serviço
    /// controla <see cref="Selecionavel"/> conforme esse modo).
    /// </summary>
    public bool GeravelNoCnab => EhTransferencia && Selecionavel && !Invalido;

    /// <summary>Entrada para o gerador (preserva a normalização feita pelo serviço).</summary>
    public PaymentInput Input { get; set; } = new();
}

/// <summary>Resultado da ação "Validar Selecionados".</summary>
public sealed class CnabValidacaoSelecionadosDto
{
    public int TotalSelecionados { get; set; }
    public decimal ValorSelecionado { get; set; }

    public int TotalValidos { get; set; }
    public decimal ValorValidos { get; set; }

    public int TotalInvalidos { get; set; }
    public decimal ValorInvalidos { get; set; }

    public int TotalCorrigidos { get; set; }
    public decimal ValorCorrigidos { get; set; }

    /// <summary>Boletos selecionados que serão ignorados na geração.</summary>
    public int TotalBoletosBloqueados { get; set; }
    public decimal ValorBoletosBloqueados { get; set; }

    public List<string> Alertas { get; set; } = new();
    public List<CnabValidacaoItemDto> Itens { get; set; } = new();
}

/// <summary>Validação por pagamento (linha do "Validar Selecionados").</summary>
public sealed class CnabValidacaoItemDto
{
    public string Chave { get; set; } = string.Empty;
    public string Origem { get; set; } = string.Empty;
    public int OrigemId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    /// <summary>Valido, Corrigido, Pendente, Invalido, Boleto.</summary>
    public string Status { get; set; } = string.Empty;

    public List<string> Observacoes { get; set; } = new();
}
