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

    /// <summary>Caminho do .zip no storage (para re-download).</summary>
    [MaxLength(400)]
    public string? CaminhoZip { get; set; }

    public List<CnabGeneratedFile> Arquivos { get; set; } = new();
    public List<CnabBatchPayment> Pagamentos { get; set; } = new();
}
