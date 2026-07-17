using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Curso : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    [Required]
    [MaxLength(150)]
    public string NomeCurso { get; set; } = string.Empty;

    public int CargaHorariaTotal { get; set; }

    public double TotalAnos { get; set; }

    public List<Tutor> Tutores { get; set; } = new();
}
