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
    public decimal SalarioBase { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TicketAlimentacao { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal OutrosProventos { get; set; }

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
        TotalProventos = SalarioBase + OutrosProventos + TicketAlimentacao + SalarioFamilia
                       + PremiacaoFixa + PremiacaoVariavel
                       + DecimoTerceiro + FeriasUmTerco;

        ValorHora = CargaHorariaSemanal > 0 ? CalcularValorHora() : 0m;
        ValorFaltas = ValorHora * HorasFalta;

        TotalDescontos = DescontoINSS + DescontoIR + PlanoSaude
                       + ValeConsignadoFGTS + ValorFaltas;

        ValorTotal = TotalProventos - TotalDescontos;
        ValorReceberPix = ValorTotal - TicketAlimentacao;
    }

    public decimal CalcularValorHora()
    {
        if (CargaHorariaSemanal <= 0)
            throw new InvalidOperationException("Carga horária semanal deve ser maior que zero.");

        decimal divisorMensal = ObterDivisorMensal(Convert.ToDecimal(CargaHorariaSemanal));
        return Math.Round(Convert.ToDecimal(SalarioBase) / divisorMensal, 2, MidpointRounding.AwayFromZero);
    }

    public decimal ObterDivisorMensal(decimal cargaHorariaSemanal)
    {
        return cargaHorariaSemanal switch
        {
            44m => 220m,
            40m => 200m,
            36m => 180m,
            30m => 150m,
            _   => (cargaHorariaSemanal / 6m) * 30m // fallback para cargas não mapeadas
        };
    }


}
