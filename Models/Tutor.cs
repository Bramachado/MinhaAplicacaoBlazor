using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Tutor
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

    [MaxLength(100)]
    public string? ChavePix { get; set; }

    [MaxLength(100)]
    public string? Banco { get; set; }

    [MaxLength(10)]
    public string? CodigoBanco { get; set; }

    [MaxLength(20)]
    public string? Agencia { get; set; }

    [MaxLength(30)]
    public string? Conta { get; set; }

    [MaxLength(150)]
    public string? NomeTitular { get; set; }

    public int TitulacaoId { get; set; }

    public Titulacao? Titulacao { get; set; }
}