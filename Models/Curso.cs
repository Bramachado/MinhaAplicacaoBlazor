using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Curso
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string NomeCurso { get; set; } = string.Empty;

    public int CargaHorariaTotal { get; set; }

    public int TotalAnos { get; set; }

    public List<Tutor> Tutores { get; set; } = new();
}
