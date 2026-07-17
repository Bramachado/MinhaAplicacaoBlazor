using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Competencia : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int Mes { get; set; }

    [Range(2000, 2100, ErrorMessage = "Informe um ano válido.")]
    public int Ano { get; set; }

    [DataType(DataType.Date)]
    public DateTime DataInicio { get; set; }

    [DataType(DataType.Date)]
    public DateTime DataFim { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Aberta";
}
