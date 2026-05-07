using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class RemessaCnab
{
    public int Id { get; set; }

    public int ConfiguracaoCnabId { get; set; }
    public ConfiguracaoCnab? ConfiguracaoCnab { get; set; }

    public int? CompetenciaId { get; set; }
    public Competencia? Competencia { get; set; }

    public int NumeroSequencial { get; set; }

    [Required, MaxLength(150)]
    public string NomeArquivo { get; set; } = string.Empty;

    public DateTime DataGeracao { get; set; } = DateTime.Now;

    public int QuantidadeRegistros { get; set; }
    public int QuantidadePagamentos { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorTotal { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Gerada";

    [MaxLength(500)]
    public string? CaminhoArquivo { get; set; }

    public string? ConteudoArquivo { get; set; }

    [MaxLength(1000)]
    public string? Observacao { get; set; }

    public List<RemessaCnabItem> Itens { get; set; } = new();
}
