using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Tutor : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

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

    public bool Ativo { get; set; } = true;

    public int TitulacaoId { get; set; }

    public Titulacao? Titulacao { get; set; }

    public int UnidadeId { get; set; }

    public Unidade? Unidade { get; set; }

    public int CursoId { get; set; }

    public Curso? Curso { get; set; }

    public int? ContaBancariaId { get; set; }

    public ContaBancaria? ContaBancaria { get; set; }
}