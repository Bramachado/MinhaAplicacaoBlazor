using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class FolhaTutorItem : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public int FolhaTutorId { get; set; }
    public FolhaTutor? FolhaTutor { get; set; }

    public int TutorId { get; set; }
    public Tutor? Tutor { get; set; }

    [MaxLength(100)]
    public string? Cargo { get; set; }

    [MaxLength(30)]
    public string? StatusPagamento { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalHorasNormais { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalHorasPraticas { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorHoraNormal { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorHoraPratica { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorAulaNormal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorAulaPratica { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotalReceber { get; set; }

    [MaxLength(3)]
    public string? Banco { get; set; }

    [MaxLength(30)]
    public string? Agencia { get; set; }

    [MaxLength(50)]
    public string? Conta { get; set; }

    [MaxLength(150)]
    public string? ChavePix { get; set; }

    [MaxLength(200)]
    public string? NomeTitularConta { get; set; }

    [MaxLength(500)]
    public string? Observacao { get; set; }

    public void Recalcular()
    {
        ValorAulaNormal = ConverterHoras(TotalHorasNormais) * ValorHoraNormal;
        ValorAulaPratica = ConverterHoras(TotalHorasPraticas) * ValorHoraPratica;
        ValorTotalReceber = ValorAulaNormal + ValorAulaPratica;
    }

    private decimal ConverterHoras(decimal horas)
    {
        int h = (int)horas;
        int minutos = (int)((horas - h) * 100);

        return h + (minutos / 60m);
    }
}