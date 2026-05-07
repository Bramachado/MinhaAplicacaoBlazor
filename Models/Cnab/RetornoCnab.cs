using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class RetornoCnab
{
    public int Id { get; set; }

    public int? RemessaCnabId { get; set; }
    public RemessaCnab? RemessaCnab { get; set; }

    [Required, MaxLength(150)]
    public string NomeArquivo { get; set; } = string.Empty;

    public DateTime DataImportacao { get; set; } = DateTime.Now;

    public string? ConteudoArquivo { get; set; }

    public int QuantidadeRegistros { get; set; }
    public int QuantidadeItensProcessados { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Importado";

    [MaxLength(1000)]
    public string? Observacao { get; set; }

    public List<RetornoCnabItem> Itens { get; set; } = new();
}
