using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.CnabBtg.Data;

/// <summary>Um arquivo .rem gerado dentro de um <see cref="CnabBatch"/>.</summary>
public class CnabGeneratedFile
{
    public int Id { get; set; }

    public int CnabBatchId { get; set; }
    public CnabBatch? CnabBatch { get; set; }

    [MaxLength(160)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>Conteúdo do .rem (linhas CRLF), guardado para re-download.</summary>
    public string Conteudo { get; set; } = string.Empty;

    public int Nsa { get; set; }
    public int QuantidadeOperacoes { get; set; }
    public int QuantidadeRegistros { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotal { get; set; }

    public bool TodasLinhas240 { get; set; }

    public DateTime CriadoEm { get; set; }
}
