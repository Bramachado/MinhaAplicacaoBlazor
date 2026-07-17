using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Empresa (tenant). É a fronteira de isolamento: cada usuário pertence a uma empresa
/// e só enxerga os registros dela. Uma empresa possui várias <see cref="Unidade"/>s.
/// </summary>
public class Empresa
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(18)]
    public string? Cnpj { get; set; }

    public bool Ativa { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.Now;
}
