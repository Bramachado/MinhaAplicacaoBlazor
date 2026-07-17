using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Unidade : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Endereco { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    public bool Ativa { get; set; } = true;

    public List<Tutor> Tutores { get; set; } = new();
}
