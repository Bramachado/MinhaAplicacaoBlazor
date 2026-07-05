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

public enum TipoConta
{
    Corrente,
    Poupanca
}

public enum TipoChavePix
{
    Nenhuma,
    Telefone,
    Email,
    CpfCnpj,
    Aleatoria
}

public class ContaBancaria : IValidatableObject
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

    /// <summary>Dígito verificador da conta (separado). Se vazio, é inferido de <see cref="Conta"/>.</summary>
    [MaxLength(2)]
    public string? DigitoConta { get; set; }

    public TipoConta TipoConta { get; set; } = TipoConta.Corrente;

    /// <summary>Tipo da chave PIX. Se <see cref="TipoChavePix.Nenhuma"/>, é inferido da chave.</summary>
    public TipoChavePix TipoChavePix { get; set; } = TipoChavePix.Nenhuma;

    /// <summary>
    /// Validação condicional conforme a <see cref="Forma"/>:
    /// PIX exige tipo da chave e chave PIX; TED exige banco, código, agência e conta.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Forma == Forma.PIX)
        {
            if (TipoChavePix == TipoChavePix.Nenhuma)
                yield return new ValidationResult(
                    "Para PIX, informe o tipo da chave.", new[] { nameof(TipoChavePix) });

            if (string.IsNullOrWhiteSpace(ChavePix))
                yield return new ValidationResult(
                    "Para PIX, informe a chave PIX.", new[] { nameof(ChavePix) });
        }
        else if (Forma == Forma.TED)
        {
            if (string.IsNullOrWhiteSpace(NomeBanco))
                yield return new ValidationResult(
                    "Para TED, informe o banco.", new[] { nameof(NomeBanco) });

            if (string.IsNullOrWhiteSpace(CodigoBanco))
                yield return new ValidationResult(
                    "Para TED, informe o código do banco.", new[] { nameof(CodigoBanco) });

            if (string.IsNullOrWhiteSpace(Agencia))
                yield return new ValidationResult(
                    "Para TED, informe a agência.", new[] { nameof(Agencia) });

            if (string.IsNullOrWhiteSpace(Conta))
                yield return new ValidationResult(
                    "Para TED, informe a conta.", new[] { nameof(Conta) });
        }
    }
}
