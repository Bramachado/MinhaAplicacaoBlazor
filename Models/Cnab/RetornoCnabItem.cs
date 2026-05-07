using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class RetornoCnabItem
{
    public int Id { get; set; }

    public int RetornoCnabId { get; set; }
    public RetornoCnab? RetornoCnab { get; set; }

    public int? RemessaCnabItemId { get; set; }
    public RemessaCnabItem? RemessaCnabItem { get; set; }

    [MaxLength(20)]
    public string? SeuNumero { get; set; }

    [MaxLength(200)]
    public string? NomeFavorecido { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorPagamento { get; set; }

    public DateTime? DataPagamento { get; set; }

    [MaxLength(10)]
    public string? CodigoOcorrencia { get; set; }

    [MaxLength(500)]
    public string? MensagemOcorrencia { get; set; }

    [MaxLength(20)]
    public string StatusProcessamento { get; set; } = "Pendente";
}
