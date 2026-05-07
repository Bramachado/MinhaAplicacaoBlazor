using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class PlanoConta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}
