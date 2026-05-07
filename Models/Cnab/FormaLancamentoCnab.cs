using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class FormaLancamentoCnab
{
    public int Id { get; set; }

    [Required, MaxLength(2)]
    public string Codigo { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Categoria { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Segmentos { get; set; }

    public bool Ativa { get; set; } = true;
}
