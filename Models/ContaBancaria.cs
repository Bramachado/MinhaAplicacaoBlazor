using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public enum TipoPessoa
{
    Fisica,
    Juridica
}

public enum Forma
{
    PIX,
    TED
}

public class ContaBancaria
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string NomeTitular { get; set; } = string.Empty;

    [Required]
    [MaxLength(18)]
    public string CpfCnpj { get; set; } = string.Empty;

    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Fisica;

    public Forma Forma { get; set; } = Forma.PIX;

    [MaxLength(100)]
    public string? ChavePix { get; set; }

    [MaxLength(3)]
    public string? CodigoBanco { get; set; }

    [MaxLength(50)]
    public string? NomeBanco { get; set; }

    [MaxLength(20)]
    public string? Agencia { get; set; }

    [MaxLength(30)]
    public string? Conta { get; set; }
}
