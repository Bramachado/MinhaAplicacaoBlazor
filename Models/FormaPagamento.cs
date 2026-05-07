using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class FormaPagamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;
}
