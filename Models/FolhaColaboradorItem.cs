using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaAplicacaoBlazor.Models;

public class FolhaColaboradorItem
{
    public int Id { get; set; }

    public int FolhaColaboradorId { get; set; }
    public FolhaColaborador? FolhaColaborador { get; set; }

    public int ColaboradorId { get; set; }
    public Colaborador? Colaborador { get; set; }

    [MaxLength(150)]
    public string? Cargo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? CargaHorariaSemanal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal SalarioBruto { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TicketAlimentacao { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal SalarioLiquidoBase { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal SalarioFamilia { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal PremiacaoFixa { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal PremiacaoVariavel { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal DecimoTerceiro { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal FeriasUmTerco { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalProventos { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorHora { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal HorasFalta { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorFaltas { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal DescontoINSS { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal DescontoIR { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal PlanoSaude { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValeConsignadoFGTS { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalDescontos { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorReceberPix { get; set; }

    [MaxLength(500)]
    public string? Observacao { get; set; }

    public void Recalcular()
    {
        TotalProventos = SalarioBruto + TicketAlimentacao + SalarioFamilia
                       + PremiacaoFixa + PremiacaoVariavel
                       + DecimoTerceiro + FeriasUmTerco;

        ValorFaltas = ValorHora * HorasFalta;

        TotalDescontos = DescontoINSS + DescontoIR + PlanoSaude
                       + ValeConsignadoFGTS + ValorFaltas;

        ValorTotal = TotalProventos - TotalDescontos;
        ValorReceberPix = ValorTotal - TicketAlimentacao;
    }
}
