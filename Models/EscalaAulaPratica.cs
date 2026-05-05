using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class EscalaAulaPratica
{
    public int Id { get; set; }

    public int TutorId { get; set; }

    public Tutor? Tutor { get; set; }

    [Required]
    [MaxLength(20)]
    public string DiaSemana { get; set; } = string.Empty;

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFim { get; set; }

    public bool Ativo { get; set; } = true;
}