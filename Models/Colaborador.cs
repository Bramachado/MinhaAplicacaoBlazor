using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class Colaborador
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Rg { get; set; }

        [MaxLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    public DateTime? DataNascimento { get; set; }

    [MaxLength(150)]
    public string? Cargo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? CargaHorariaSemanal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? SalarioBase { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? TicketAlimentacao { get; set; }

    public bool Ativo { get; set; } = true;

    public int UnidadeId { get; set; }

    public Unidade? Unidade { get; set; }

    public int? ContaBancariaId { get; set; }

    public ContaBancaria? ContaBancaria { get; set; }
}
