namespace MinhaAplicacaoBlazor.Models.Reports;

public class CompetenciaSelecaoDto
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TextoExibicao => $"{Mes:00}/{Ano} - {Status}";
}

public class ResumoFinanceiroCardDto
{
    public string Titulo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string CssClass { get; set; } = "border-secondary";
    public string? Descricao { get; set; }
}

public class RelatorioGeralDespesasDto
{
    public List<RelatorioGeralDespesaItemDto> Itens { get; set; } = new();
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public decimal TotalDespesasFixas { get; set; }
    public decimal TotalDespesasVariaveis { get; set; }
}

public class RelatorioGeralDespesaItemDto
{
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? Unidade { get; set; }
    public string PlanoConta { get; set; } = string.Empty;
    public string? Fornecedor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? FormaPagamento { get; set; }
    public string TipoLancamento { get; set; } = string.Empty;
    public string? TipoDespesa { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RelatorioAgrupadoDto<TLinha>
{
    public List<TLinha> Linhas { get; set; } = new();
    public int Quantidade { get; set; }
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
}

public class RelatorioPorUnidadeDto : RelatorioAgrupadoDto<RelatorioPorUnidadeLinhaDto> { }

public class RelatorioPorUnidadeLinhaDto
{
    public string Unidade { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioPorPlanoContaDto : RelatorioAgrupadoDto<RelatorioPorPlanoContaLinhaDto> { }

public class RelatorioPorPlanoContaLinhaDto
{
    public string PlanoConta { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioPorFormaPagamentoDto : RelatorioAgrupadoDto<RelatorioPorFormaPagamentoLinhaDto> { }

public class RelatorioPorFormaPagamentoLinhaDto
{
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioFolhaColaboradorDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RelatorioFolhaColaboradorItemDto> Itens { get; set; } = new();
    public int QuantidadeColaboradores => Itens.Count;
    public decimal TotalProventos => Itens.Sum(i => i.TotalProventos);
    public decimal TotalDescontos => Itens.Sum(i => i.TotalDescontos);
    public decimal TotalLiquido => Itens.Sum(i => i.ValorTotal);
    public decimal TotalReceberPix => Itens.Sum(i => i.ValorReceberPix);
}

public class RelatorioFolhaColaboradorItemDto
{
    public string Colaborador { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string? Cargo { get; set; }
    public decimal? CargaHorariaSemanal { get; set; }
    public decimal SalarioBruto { get; set; }
    public decimal TicketAlimentacao { get; set; }
    public decimal PremiacaoFixa { get; set; }
    public decimal PremiacaoVariavel { get; set; }
    public decimal DecimoTerceiro { get; set; }
    public decimal FeriasUmTerco { get; set; }
    public decimal TotalProventos { get; set; }
    public decimal DescontoINSS { get; set; }
    public decimal DescontoIR { get; set; }
    public decimal PlanoSaude { get; set; }
    public decimal ValeConsignadoFGTS { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorReceberPix { get; set; }
    public string? Observacao { get; set; }
}

public class RelatorioFolhaTutorDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RelatorioFolhaTutorItemDto> Itens { get; set; } = new();
    public int QuantidadeTutores => Itens.Count;
    public decimal TotalHorasNormais => Itens.Sum(i => i.TotalHorasNormais);
    public decimal TotalHorasPraticas => Itens.Sum(i => i.TotalHorasPraticas);
    public decimal TotalHoras => TotalHorasNormais + TotalHorasPraticas;
    public decimal TotalAPagar => Itens.Sum(i => i.ValorTotalReceber);
}

public class RelatorioFolhaTutorItemDto
{
    public string Tutor { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string? Titulacao { get; set; }
    public string? Cargo { get; set; }
    public string? StatusPagamento { get; set; }
    public decimal TotalHorasNormais { get; set; }
    public decimal TotalHorasPraticas { get; set; }
    public decimal ValorHoraNormal { get; set; }
    public decimal ValorHoraPratica { get; set; }
    public decimal ValorAulaNormal { get; set; }
    public decimal ValorAulaPratica { get; set; }
    public decimal ValorTotalReceber { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePix { get; set; }
    public string? NomeTitularConta { get; set; }
    public string? Observacao { get; set; }
}

public class ResumoDespesaGeralDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public List<ResumoLinhaDto> Entradas { get; set; } = new();
    public decimal TotalEntradas { get; set; }
    public List<ResumoGrupoDto> SaidasPorPlanoConta { get; set; } = new();
    public decimal TotalSaidas { get; set; }
    public decimal TotalFolhaColaboradores { get; set; }
    public decimal TotalFolhaTutores { get; set; }
    public decimal SaldoFinal => TotalEntradas - TotalSaidas;
    public decimal TotalDespesasFixas { get; set; }
    public decimal TotalDespesasVariaveis { get; set; }
}

public class ResumoLinhaDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class ResumoGrupoDto
{
    public string PlanoConta { get; set; } = string.Empty;
    public List<ResumoLinhaDto> Lancamentos { get; set; } = new();
    public decimal Total { get; set; }
}
