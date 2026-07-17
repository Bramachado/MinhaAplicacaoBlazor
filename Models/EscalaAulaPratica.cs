using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class EscalaAulaPratica : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public int TutorId { get; set; }

    public Tutor? Tutor { get; set; }

    [Required]
    [MaxLength(7)]
    [RegularExpression(@"^(0[1-9]|1[0-2])/\d{4}$", ErrorMessage = "Informe o mês no formato MM/AAAA.")]
    public string MesAulaPratica { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string DiaSemana { get; set; } = string.Empty;

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFim { get; set; }

    [MaxLength(100)]
    public string? CursoTurma { get; set; }

    [MaxLength(50)]
    public string? SalaLab { get; set; }

    [MaxLength(500)]
    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;
}