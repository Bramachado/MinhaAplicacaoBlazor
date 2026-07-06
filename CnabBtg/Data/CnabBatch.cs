using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.CnabBtg.Data;

/// <summary>
/// Registro de uma geração de CNAB (lote de geração). Persiste os parâmetros, os
/// totais, os arquivos gerados e os pagamentos incluídos/rejeitados. Não marca
/// pagamentos como "pago" — apenas registra a inclusão no CNAB.
/// </summary>
public class CnabBatch
{
    public int Id { get; set; }

    public int? CompetenciaId { get; set; }

    [MaxLength(30)]
    public string EmpresaPagadora { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Ambiente { get; set; } = "TESTE";

    [MaxLength(30)]
    public string? Convenio { get; set; }

    public int NsaInicial { get; set; }
    public int NsaFinal { get; set; }

    [MaxLength(150)]
    public string NomeBaseArquivo { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? TipoPagamentoPrincipal { get; set; }

    public bool SeparadoPorForma { get; set; }

    [MaxLength(20)]
    public string TratamentoInvalidos { get; set; } = "Remover";

    public int TotalSelecionados { get; set; }
    public int TotalValidos { get; set; }
    public int TotalInvalidos { get; set; }
    public int TotalCorrigidos { get; set; }
    public int TotalPendentes { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotal { get; set; }

    /// <summary>GERADO, BLOQUEADO, CANCELADO.</summary>
    [MaxLength(20)]
    public string Status { get; set; } = "GERADO";

    public DateTime GeradoEm { get; set; }

    [MaxLength(200)]
    public string? GeradoPor { get; set; }

    /// <summary>Auditoria completa em JSON (persistida para reimpressão/rastreio).</summary>
    public string? AuditoriaJson { get; set; }

    // === Campos de conferência da tela "Gerenciar CNAB" ===

    /// <summary>Tipo da geração: TRANSFERENCIA (padrão). BOLETO reservado para módulo J/J-52 futuro.</summary>
    [MaxLength(20)]
    public string TipoGeracao { get; set; } = "TRANSFERENCIA";

    /// <summary>Banco de cadastro/origem filtrado na conferência, se aplicável.</summary>
    public int? BancoCadastroId { get; set; }

    [MaxLength(80)]
    public string? BancoCadastroNome { get; set; }

    /// <summary>Total da competência (todos os pagamentos, inclusive boletos e inválidos).</summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotalCompetencia { get; set; }

    /// <summary>Total dos pagamentos selecionados pelo usuário.</summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotalSelecionado { get; set; }

    /// <summary>Total efetivamente gravado no(s) .rem.</summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotalGerado { get; set; }

    /// <summary>Total de boletos bloqueados (não entram no .rem).</summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotalBoletos { get; set; }

    /// <summary>Total dos inválidos removidos/bloqueados.</summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotalInvalidos { get; set; }

    public int QuantidadeTotalCompetencia { get; set; }
    public int QuantidadeSelecionada { get; set; }
    public int QuantidadeBoletos { get; set; }

    /// <summary>Caminho do .zip no storage (para re-download).</summary>
    [MaxLength(400)]
    public string? CaminhoZip { get; set; }

    public List<CnabGeneratedFile> Arquivos { get; set; } = new();
    public List<CnabBatchPayment> Pagamentos { get; set; } = new();
}
