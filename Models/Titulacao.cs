using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Titulacao : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public decimal ValorHoraAulaNormal { get; set; }

    public decimal ValorHoraAulaPratica { get; set; }

    public List<Tutor> Tutores { get; set; } = new();
}