using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Banco da empresa (conta pagadora). Cadastro simples usado para indicar de qual
/// banco sai o pagamento de despesas (folha de fornecedores) e para as entradas.
/// </summary>
public class Banco
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string NomeBanco { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Descricao { get; set; }
}
