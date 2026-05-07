using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class RemessaCnabItem
{
    public int Id { get; set; }

    public int RemessaCnabId { get; set; }
    public RemessaCnab? RemessaCnab { get; set; }

    public int? LancamentoFinanceiroId { get; set; }
    public LancamentoFinanceiro? LancamentoFinanceiro { get; set; }

    public int? FolhaColaboradorItemId { get; set; }
    public FolhaColaboradorItem? FolhaColaboradorItem { get; set; }

    public int? FolhaTutorItemId { get; set; }
    public FolhaTutorItem? FolhaTutorItem { get; set; }

    public int? FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    public int FormaLancamentoCnabId { get; set; }
    public FormaLancamentoCnab? FormaLancamentoCnab { get; set; }

    [Required, MaxLength(200)]
    public string NomeFavorecido { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? CPF_CNPJ_Favorecido { get; set; }

    [MaxLength(10)]
    public string? BancoFavorecido { get; set; }

    [MaxLength(5)]
    public string? AgenciaFavorecido { get; set; }

    [MaxLength(1)]
    public string? AgenciaDVFavorecido { get; set; }

    [MaxLength(12)]
    public string? ContaFavorecido { get; set; }

    [MaxLength(1)]
    public string? ContaDVFavorecido { get; set; }

    [MaxLength(150)]
    public string? ChavePix { get; set; }

    [MaxLength(20)]
    public string? TipoChavePix { get; set; }

    [MaxLength(48)]
    public string? CodigoBarras { get; set; }

    public DateTime DataPagamento { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal ValorPagamento { get; set; }

    [MaxLength(20)]
    public string SeuNumero { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pendente";

    [MaxLength(10)]
    public string? CodigoOcorrencia { get; set; }

    [MaxLength(500)]
    public string? MensagemOcorrencia { get; set; }
}
