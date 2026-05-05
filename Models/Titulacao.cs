using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Titulacao
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public decimal ValorHoraAulaNormal { get; set; }

    public decimal ValorHoraAulaPratica { get; set; }

    public List<Tutor> Tutores { get; set; } = new();
}